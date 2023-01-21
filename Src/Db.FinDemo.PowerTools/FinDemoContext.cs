#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Db.FinDemo.PowerTools.Models;

public partial class FinDemoContext
{
  readonly string _sqlConnectionString = "<Not Initialized!!!>";//todo: if not done: remove warnig and ... in protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.

  public FinDemoContext(string connectoinString)
  {
    _sqlConnectionString = connectoinString;
#if DEBUG_
      if (Debugger.IsAttached && System.Environment.UserDomainName == "RAZER1")
      {
        Debugger.Break();
        Database.EnsureCreated();
      }
#endif
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });
      optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.                                                                                                                                                                                                                                 
    }
  }
}