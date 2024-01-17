﻿using MinFin7.MNT.Stores;
using MinFin7.MNT.VM.Services;
using MinFin7.MNT.VM.VMs;

namespace MinFin7.MNT.Services;
public class LayoutNavSvc<TVM> : INavSvc where TVM : BaseMinVM
{
  readonly NavigationStore _navigationStore;
  readonly Func<NavBarVM> _createNavBarVM;
  readonly Func<TVM> _createVM;

  public LayoutNavSvc(NavigationStore navigationStore, Func<TVM> createVM, Func<NavBarVM> createNavBarVM)
  {
    _navigationStore = navigationStore;
    _createVM = createVM;
    _createNavBarVM = createNavBarVM;
  }

  public async void Navigate()
  {
    if (_navigationStore.CurrentVM is not null && 
      ((MinFin7.MNT.VM.VMs.LayoutVM)_navigationStore.CurrentVM).ContentVM is not null &&
      await ((LayoutVM)_navigationStore.CurrentVM).ContentVM.WrapAsync() == false)
      return;

    _navigationStore.CurrentVM = new LayoutVM(_createNavBarVM(), _createVM());
    await ((LayoutVM)_navigationStore.CurrentVM).ContentVM.InitAsync();
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