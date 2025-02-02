using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using WalletWasabi.Fluent.Models;

namespace WalletWasabi.Fluent.Behaviors;

public class FlyoutSuggestionBehavior : AttachedToVisualTreeBehavior<Control>
{
	public static readonly StyledProperty<string> ContentProperty = AvaloniaProperty.Register<FlyoutSuggestionBehavior, string>(nameof(Content));

	public static readonly StyledProperty<IDataTemplate> HintTemplateProperty = AvaloniaProperty.Register<FlyoutSuggestionBehavior, IDataTemplate>(nameof(HintTemplate));

	public static readonly StyledProperty<Control?> TargetProperty = AvaloniaProperty.Register<FlyoutSuggestionBehavior, Control?>(nameof(Target));

	public static readonly StyledProperty<FlyoutPlacementMode> PlacementModeProperty = AvaloniaProperty.Register<FlyoutSuggestionBehavior, FlyoutPlacementMode>(nameof(PlacementMode));

	private readonly Flyout _flyout;

	public FlyoutSuggestionBehavior()
	{
		_flyout = new Flyout { ShowMode = FlyoutShowMode.Transient };
	}

	public FlyoutPlacementMode PlacementMode
	{
		get => GetValue(PlacementModeProperty);
		set => SetValue(PlacementModeProperty, value);
	}

	public string Content
	{
		get => GetValue(ContentProperty);
		set => SetValue(ContentProperty, value);
	}

	public IDataTemplate HintTemplate
	{
		get => GetValue(HintTemplateProperty);
		set => SetValue(HintTemplateProperty, value);
	}

	public Control? Target
	{
		get => GetValue(TargetProperty);
		set => SetValue(TargetProperty, value);
	}

	protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
	{
		var targets = this
			.WhenAnyValue(x => x.Target)
			.WhereNotNull();

		Displayer(targets).DisposeWith(disposable);
		Hider(targets).DisposeWith(disposable);

		Target ??= AssociatedObject as TextBox;

		this.WhenAnyValue(x => x.PlacementMode)
			.Do(x => _flyout.Placement = x)
			.Subscribe()
			.DisposeWith(disposable);
	}

	private IDisposable Hider(IObservable<Control> targets)
	{
		return targets
			.Select(x => Observable.FromEventPattern(x, nameof(x.LostFocus)))
			.Switch()
			.Select(x => (TextBox?) x.Sender)
			.Do(_ => _flyout.Hide())
			.Subscribe();
	}

	private IDisposable Displayer(IObservable<Control> targets)
	{
		return targets
			.Select(x => Observable.FromEventPattern(x, nameof(x.GotFocus)))
			.Switch()
			.Select(x => (TextBox?) x.Sender)
			.WithLatestFrom(this.WhenAnyValue(x => x.Content))
			.Where(tuple => !string.IsNullOrWhiteSpace(tuple.Second) && tuple.First?.Text != tuple.Second)
			.Select(tuple => CreateSuggestion(tuple.First, tuple.Second))
			.Do(ShowHint)
			.Subscribe();
	}

	private Suggestion CreateSuggestion(TextBox? textBox, string content)
	{
		return new Suggestion(
			content,
			() =>
			{
				if (textBox != null)
				{
					textBox.Text = content;
				}

				_flyout.Hide();
			});
	}

	private void ShowHint(Suggestion suggestion)
	{
		_flyout.Content = new ContentControl { ContentTemplate = HintTemplate, Content = suggestion };
		if (Target != null)
		{
			_flyout.ShowAt(Target);
		}
	}
}
