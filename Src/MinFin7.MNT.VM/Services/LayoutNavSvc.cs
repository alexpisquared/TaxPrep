namespace MinFin7.MNT.VM.Services;
public class LayoutNavSvc<TVM> : INavSvc where TVM : BaseMinVM
{
  readonly NavigationStore _store;
  readonly Func<NavBarVM> _createNB;
  readonly Func<TVM> _createVM;

  public LayoutNavSvc(NavigationStore store, Func<TVM> createVM, Func<NavBarVM> createNB) => (_store, _createVM, _createNB) = (store, createVM, createNB);

  public async void Navigate()
  {
    Console.Beep(7000, 50);

    if ((_store.CurrentVM as LayoutVM)?.ContentVM is not null && await ((LayoutVM)_store.CurrentVM).ContentVM.WrapAsync() == false)
      return;

    _store.CurrentVM = new LayoutVM(_createNB(), _createVM());

    _ = await ((LayoutVM)_store.CurrentVM).ContentVM.InitAsync();
  }
}

public class Page00NavSvc : LayoutNavSvc<Page00VM> { public Page00NavSvc(NavigationStore ns, Func<Page00VM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
public class Page01NavSvc : LayoutNavSvc<Page01VM> { public Page01NavSvc(NavigationStore ns, Func<Page01VM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
public class MdiMenuNavSvc : LayoutNavSvc<MdiMenuVM> { public MdiMenuNavSvc(NavigationStore ns, Func<MdiMenuVM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
//public class Page02NavSvc : LayoutNavSvc<Page02VM> { public Page02NavSvc(NavigationStore ns, Func<Page02VM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
//public class Page03NavSvc : LayoutNavSvc<Page03VM> { public Page03NavSvc(NavigationStore ns, Func<Page03VM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
//public class Page04NavSvc : LayoutNavSvc<Page04VM> { public Page04NavSvc(NavigationStore ns, Func<Page04VM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
//public class Page05NavSvc : LayoutNavSvc<Page05VM> { public Page05NavSvc(NavigationStore ns, Func<Page05VM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }
public class EmailDetailNavSvc : LayoutNavSvc<EmailDetailVM> { public EmailDetailNavSvc(NavigationStore ns, Func<EmailDetailVM> vm, Func<NavBarVM> nb) : base(ns, vm, nb) { } }