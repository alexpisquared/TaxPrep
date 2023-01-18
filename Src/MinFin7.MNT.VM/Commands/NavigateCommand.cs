using MinFin7.MNT.VM.Services;

namespace MinFin7.MNT.Commands;

public class NavigateCommand : CommandBase
{
  readonly INavSvc _navigationService;

  public NavigateCommand(INavSvc navigationService) => _navigationService = navigationService;

  public override void Execute(object? parameter) => _navigationService.Navigate();
}
