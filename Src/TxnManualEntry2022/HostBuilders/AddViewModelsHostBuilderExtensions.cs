
namespace TxnManualEntry2022.HostBuilders;

public static class AddViewModelsHostBuilderExtensions
{
  public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
  {
    hostBuilder.ConfigureServices(services =>
    {
      services.AddTransient((s) => CreateAcntTxnListingVM(s));
      services.AddSingleton<Func<AcntTxnListingVM>>((s) => () => s.GetRequiredService<AcntTxnListingVM>());
      services.AddSingleton<NavigationService<AcntTxnListingVM>>();

      services.AddTransient<MakeAcntTxnVM>();
      services.AddSingleton<Func<MakeAcntTxnVM>>((s) => () => s.GetRequiredService<MakeAcntTxnVM>()); // https://youtu.be/dgJ1nS2CLpQ?list=PLA8ZIAm2I03hS41Fy4vFpRw8AdYNBXmNm&t=549
      services.AddSingleton<NavigationService<MakeAcntTxnVM>>();
    });

    return hostBuilder;
  }

 static AcntTxnListingVM CreateAcntTxnListingVM(IServiceProvider s) =>
  AcntTxnListingVM.LoadViewModel(
    s.GetRequiredService<BankAccountStore>(),
    s.GetRequiredService<NavigationService<MakeAcntTxnVM>>());



}
