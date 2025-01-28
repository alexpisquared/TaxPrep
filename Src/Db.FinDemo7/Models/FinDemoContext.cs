using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Db.FinDemo7.Models;

public partial class FinDemoContext : DbContext
{
    public FinDemoContext()
    {
    }

    public FinDemoContext(DbContextOptions<FinDemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BalAmtHist> BalAmtHists { get; set; }

    public virtual DbSet<Contribution> Contributions { get; set; }

    public virtual DbSet<ExpenseGroup> ExpenseGroups { get; set; }

    public virtual DbSet<FinEngine> FinEngines { get; set; }

    public virtual DbSet<LkuStatus> LkuStatuses { get; set; }

    public virtual DbSet<TxCategory> TxCategories { get; set; }

    public virtual DbSet<TxCore> TxCores { get; set; }

    public virtual DbSet<TxCoreAudit> TxCoreAudits { get; set; }

    public virtual DbSet<TxCoreCiVi> TxCoreCiVis { get; set; }

    public virtual DbSet<TxCoreDetail> TxCoreDetails { get; set; }

    public virtual DbSet<TxCorePcMc> TxCorePcMcs { get; set; }

    public virtual DbSet<TxCoreTdCt> TxCoreTdCts { get; set; }

    public virtual DbSet<TxCoreV2> TxCoreV2s { get; set; }

    public virtual DbSet<TxEnvelope> TxEnvelopes { get; set; }

    public virtual DbSet<TxMoneySrc> TxMoneySrcs { get; set; }

    public virtual DbSet<TxSupplier> TxSuppliers { get; set; }

    public virtual DbSet<UnitPrice> UnitPrices { get; set; }

    public virtual DbSet<VwDupe> VwDupes { get; set; }

    public virtual DbSet<VwDupesDetail> VwDupesDetails { get; set; }

    public virtual DbSet<VwExp2010201120122013201420152016Vs2017b> VwExp2010201120122013201420152016Vs2017bs { get; set; }

    public virtual DbSet<VwExp201020112012201320142015Vs2016b> VwExp201020112012201320142015Vs2016bs { get; set; }

    public virtual DbSet<VwExp20102011201220132014Vs2015b> VwExp20102011201220132014Vs2015bs { get; set; }

    public virtual DbSet<VwExp2010201120122013Vs2014b> VwExp2010201120122013Vs2014bs { get; set; }

    public virtual DbSet<VwExp2010201120122013Vs2014bOld> VwExp2010201120122013Vs2014bOlds { get; set; }

    public virtual DbSet<VwExp201020112012Vs2013b> VwExp201020112012Vs2013bs { get; set; }

    public virtual DbSet<VwExp20102011Vs2012> VwExp20102011Vs2012s { get; set; }

    public virtual DbSet<VwExp20102011Vs2012b> VwExp20102011Vs2012bs { get; set; }

    public virtual DbSet<VwExp2010Vs2011> VwExp2010Vs2011s { get; set; }

    public virtual DbSet<VwExpHistVs2018> VwExpHistVs2018s { get; set; }

    public virtual DbSet<VwExpHistVs2018Alx> VwExpHistVs2018Alxes { get; set; }

    public virtual DbSet<VwExpHistVs2018Mei> VwExpHistVs2018Meis { get; set; }

    public virtual DbSet<VwExpHistVs2018Ndn> VwExpHistVs2018Ndns { get; set; }

    public virtual DbSet<VwExpHistVs2018Zoe> VwExpHistVs2018Zoes { get; set; }

    public virtual DbSet<VwExpHistVs2019> VwExpHistVs2019s { get; set; }

    public virtual DbSet<VwExpHistVsLast> VwExpHistVsLasts { get; set; }

    public virtual DbSet<VwTaxLiqReport> VwTaxLiqReports { get; set; }

    public virtual DbSet<VwTaxLiqReport2010> VwTaxLiqReport2010s { get; set; }

    public virtual DbSet<VwTaxLiqReport2011> VwTaxLiqReport2011s { get; set; }

    public virtual DbSet<VwTaxLiqReport2012> VwTaxLiqReport2012s { get; set; }

    public virtual DbSet<VwTaxLiqReport20121> VwTaxLiqReport2012s1 { get; set; }

    public virtual DbSet<VwTaxLiqReport2014> VwTaxLiqReport2014s { get; set; }

    public virtual DbSet<VwTaxLiqReport2014V2> VwTaxLiqReport2014V2s { get; set; }

    public virtual DbSet<VwTaxLiqReport2103> VwTaxLiqReport2103s { get; set; }

    public virtual DbSet<VwTaxLiqReportAlx> VwTaxLiqReportAlxes { get; set; }

    public virtual DbSet<VwTaxLiqReportMei> VwTaxLiqReportMeis { get; set; }

    public virtual DbSet<VwTxCore> VwTxCores { get; set; }

    public virtual DbSet<ZTest> ZTests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BalAmtHist>(entity =>
        {
            entity.ToTable("BalAmtHist");

            entity.Property(e => e.AsOfDate).HasColumnType("datetime");
            entity.Property(e => e.BalAmt).HasColumnType("money");
            entity.Property(e => e.BalTpe)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasMaxLength(2000)
                .IsUnicode(false);

            entity.HasOne(d => d.TxMoneySrc).WithMany(p => p.BalAmtHists)
                .HasForeignKey(d => d.TxMoneySrcId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BalAmtHist_TxSource");
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.ToTable("Contribution");

            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.TranAmount).HasColumnType("money");
            entity.Property(e => e.UnitPrice).HasColumnType("money");

            entity.HasOne(d => d.FinEngine).WithMany(p => p.Contributions)
                .HasForeignKey(d => d.FinEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contribution_FinEngine");
        });

        modelBuilder.Entity<ExpenseGroup>(entity =>
        {
            entity.ToTable("ExpenseGroup");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Note).IsUnicode(false);
        });

        modelBuilder.Entity<FinEngine>(entity =>
        {
            entity.ToTable("FinEngine");

            entity.Property(e => e.AccountNumber)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.DescrName)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Notes).IsUnicode(false);
        });

        modelBuilder.Entity<LkuStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("LkuStatus");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TxCategory>(entity =>
        {
            entity.HasKey(e => e.IdTxt);

            entity.ToTable("TxCategory");

            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("tla");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeductiblePercentage).HasDefaultValue(1.0);
            entity.Property(e => e.ExpGroupId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Other");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");

            entity.HasOne(d => d.ExpGroup).WithMany(p => p.TxCategories)
                .HasForeignKey(d => d.ExpGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCategory_ExpenseGroup");
        });

        modelBuilder.Entity<TxCore>(entity =>
        {
            entity.ToTable("TxCore");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.History).IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.ProductService)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasDefaultValue("-");
            entity.Property(e => e.TxAmount).HasColumnType("money");
            entity.Property(e => e.TxAmountOrg).HasColumnType("money");
            entity.Property(e => e.TxCategoryId).HasDefaultValue(-1);
            entity.Property(e => e.TxCategoryIdTxt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("UnKn");
            entity.Property(e => e.TxDate).HasColumnType("datetime");
            entity.Property(e => e.TxSupplierId).HasDefaultValue(3);

            entity.HasOne(d => d.TxCategoryIdTxtNavigation).WithMany(p => p.TxCores)
                .HasForeignKey(d => d.TxCategoryIdTxt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCore_TxCategory1");

            entity.HasOne(d => d.TxEnvelope).WithMany(p => p.TxCores)
                .HasForeignKey(d => d.TxEnvelopeId)
                .HasConstraintName("FK_TxCore_TxEnvelope");

            entity.HasOne(d => d.TxMoneySrc).WithMany(p => p.TxCores)
                .HasForeignKey(d => d.TxMoneySrcId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCore_TxSource");

            entity.HasOne(d => d.TxSupplier).WithMany(p => p.TxCores)
                .HasForeignKey(d => d.TxSupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCore_TxSupplier");
        });

        modelBuilder.Entity<TxCoreAudit>(entity =>
        {
            entity.ToTable("TxCoreAudit");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.UpToDateTxAmount).HasColumnType("money");
        });

        modelBuilder.Entity<TxCoreCiVi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TxCoreCiVi");

            entity.ToTable("TxCore_CiVi");

            entity.HasIndex(e => e.FitId, "IX_TxCore_CiVi").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnType("money");
            entity.Property(e => e.Debit).HasColumnType("money");
            entity.Property(e => e.Details)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.DtPosted).HasColumnType("datetime");
            entity.Property(e => e.F6)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.FitId)
                .HasMaxLength(400)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
        });

        modelBuilder.Entity<TxCoreDetail>(entity =>
        {
            entity.ToTable("TxCoreDetail");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);

            entity.HasOne(d => d.TxCore).WithMany(p => p.TxCoreDetails)
                .HasForeignKey(d => d.TxCoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCoreDetail_TxCore");
        });

        modelBuilder.Entity<TxCorePcMc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TxCorePcMc");

            entity.ToTable("TxCore_PcMc");

            entity.HasIndex(e => e.ReferenceNumber, "IX_TxCore_PcMc").IsUnique();

            entity.Property(e => e.BillingAmount)
                .HasColumnType("money")
                .HasColumnName("Billing Amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DebitCreditFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Debit/Credit Flag");
            entity.Property(e => e.Merchant)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MerchantCity)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Merchant City");
            entity.Property(e => e.MerchantState)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Merchant State");
            entity.Property(e => e.MerchantZip)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Merchant Zip");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.PostingDate)
                .HasColumnType("datetime")
                .HasColumnName("Posting Date");
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Reference Number");
            entity.Property(e => e.SicmccCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SICMCC Code");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("datetime")
                .HasColumnName("Transaction Date");
        });

        modelBuilder.Entity<TxCoreTdCt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TxCoreTD");

            entity.ToTable("TxCore_TdCt");

            entity.HasIndex(e => new { e.TxDate, e.TxDescrn, e.TxAmountBlnc }, "IX_TxCore_TdCt").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.TxAmountBlnc).HasColumnType("money");
            entity.Property(e => e.TxAmountCrt).HasColumnType("money");
            entity.Property(e => e.TxAmountDbt).HasColumnType("money");
            entity.Property(e => e.TxDate).HasColumnType("datetime");
            entity.Property(e => e.TxDescrn)
                .HasMaxLength(400)
                .IsUnicode(false)
                .HasDefaultValue("-");
        });

        modelBuilder.Entity<TxCoreV2>(entity =>
        {
            entity.ToTable("TxCoreV2");

            entity.HasIndex(e => e.FitId, "IX_TxCoreV2").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurBalance)
                .HasDefaultValue(0m)
                .HasColumnType("money");
            entity.Property(e => e.FitId)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.HstTakenAt).HasColumnType("datetime");
            entity.Property(e => e.MemoPp)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MemoPP");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.TxAmount).HasColumnType("money");
            entity.Property(e => e.TxCategoryIdTxt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("UnKn");
            entity.Property(e => e.TxDate).HasColumnType("datetime");
            entity.Property(e => e.TxDetail)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.TxCategoryIdTxtNavigation).WithMany(p => p.TxCoreV2s)
                .HasForeignKey(d => d.TxCategoryIdTxt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCoreV2_TxCategory1");

            entity.HasOne(d => d.TxMoneySrc).WithMany(p => p.TxCoreV2s)
                .HasForeignKey(d => d.TxMoneySrcId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TxCoreV2_TxSource");
        });

        modelBuilder.Entity<TxEnvelope>(entity =>
        {
            entity.ToTable("TxEnvelope");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Note).IsUnicode(false);
            entity.Property(e => e.Total).HasColumnType("money");
        });

        modelBuilder.Entity<TxMoneySrc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TxSource");

            entity.ToTable("TxMoneySrc");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Fla)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("UnKn");
            entity.Property(e => e.IniBalance).HasColumnType("money");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Owner)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<TxSupplier>(entity =>
        {
            entity.ToTable("TxSupplier");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
        });

        modelBuilder.Entity<UnitPrice>(entity =>
        {
            entity.ToTable("UnitPrice");

            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.Value).HasColumnType("money");

            entity.HasOne(d => d.FinEngine).WithMany(p => p.UnitPrices)
                .HasForeignKey(d => d.FinEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnitPrice_FinEngine");
        });

        modelBuilder.Entity<VwDupe>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Dupes");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.History).IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.ProductService)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TxAmount).HasColumnType("money");
            entity.Property(e => e.TxAmountOrg).HasColumnType("money");
            entity.Property(e => e.TxCategoryIdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TxDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VwDupesDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Dupes_Detail");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.History).IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.MoneySrc)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.ProductService)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Supplier)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TxAmount).HasColumnType("money");
            entity.Property(e => e.TxCategoryIdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TxDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VwExp2010201120122013201420152016Vs2017b>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_2012_2013_2014_2015_2016_vs_2017b");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp201020112012201320142015Vs2016b>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_2012_2013_2014_2015_vs_2016b");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp20102011201220132014Vs2015b>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_2012_2013_2014_vs_2015b");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp2010201120122013Vs2014b>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_2012_2013_vs_2014b");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("money");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp2010201120122013Vs2014bOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_2012_2013_vs_2014b_OLD");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("money");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp201020112012Vs2013b>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_2012_vs_2013b");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("money");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp20102011Vs2012>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_vs_2012");

            entity.Property(e => e.Avg2010to2011).HasColumnType("numeric(21, 5)");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseAvg2010to2011to2012).HasColumnType("numeric(38, 15)");
        });

        modelBuilder.Entity<VwExp20102011Vs2012b>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_2011_vs_2012b");

            entity.Property(e => e.Avg2010to2011).HasColumnType("numeric(21, 5)");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Max2010to2011).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseAvg2010to2011to2012).HasColumnType("numeric(38, 18)");
            entity.Property(e => e.PercIncreaseMax2010to2011to2012).HasColumnType("numeric(38, 19)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExp2010Vs2011>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_2010_vs_2011");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
        });

        modelBuilder.Entity<VwExpHistVs2018>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_2018");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExpHistVs2018Alx>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_2018_Alx");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExpHistVs2018Mei>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_2018_Mei");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExpHistVs2018Ndn>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_2018_Ndn");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExpHistVs2018Zoe>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_2018_Zoe");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExpHistVs2019>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_2019");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.Exp2019).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.PercIncreaseMaxPrevToCurrent).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwExpHistVsLast>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Exp_Hist_vs_Last");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Cur2Max).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.Exp2010).HasColumnType("money");
            entity.Property(e => e.Exp2011).HasColumnType("money");
            entity.Property(e => e.Exp2012).HasColumnType("money");
            entity.Property(e => e.Exp2013).HasColumnType("money");
            entity.Property(e => e.Exp2014).HasColumnType("money");
            entity.Property(e => e.Exp2015).HasColumnType("money");
            entity.Property(e => e.Exp2016).HasColumnType("money");
            entity.Property(e => e.Exp2017).HasColumnType("money");
            entity.Property(e => e.Exp2018).HasColumnType("money");
            entity.Property(e => e.Exp2019).HasColumnType("money");
            entity.Property(e => e.Exp2020).HasColumnType("money");
            entity.Property(e => e.Exp2021).HasColumnType("money");
            entity.Property(e => e.Exp2022).HasColumnType("money");
          entity.Property(e => e.Exp2023).HasColumnType("money");
          entity.Property(e => e.Exp2024).HasColumnType("money");
            entity.Property(e => e.ExpenseGroupId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaxPrev).HasColumnType("money");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.TaxLiq).HasColumnName("TaxLiq##");
        });

        modelBuilder.Entity<VwTaxLiqReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstTx).HasColumnType("datetime");
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(28)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport2010>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2010");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport2011>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2011");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport2012>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2012");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport20121>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2012_");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport2014>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2014");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstTx).HasColumnType("datetime");
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport2014V2>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2014_V2");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstTx).HasColumnType("datetime");
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReport2103>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_2103");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstTx).HasColumnType("datetime");
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(19)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReportAlx>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_Alx");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstTx).HasColumnType("datetime");
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.Owner)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(28)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTaxLiqReportMei>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TaxLiqReport_Mei");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstTx).HasColumnType("datetime");
            entity.Property(e => e.Group).IsUnicode(false);
            entity.Property(e => e.LastTx).HasColumnType("datetime");
            entity.Property(e => e.Owner)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.PartCalcShow)
                .HasMaxLength(28)
                .IsUnicode(false);
            entity.Property(e => e.TlNumber).HasColumnName("TL_Number");
        });

        modelBuilder.Entity<VwTxCore>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_TxCore");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CurBalance).HasColumnType("money");
            entity.Property(e => e.FitId)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.History).IsUnicode(false);
            entity.Property(e => e.MemoPp)
                .IsUnicode(false)
                .HasColumnName("MemoPP");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.ProductService)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TxAmount).HasColumnType("money");
            entity.Property(e => e.TxAmountOrg).HasColumnType("money");
            entity.Property(e => e.TxCategoryIdTxt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TxDate).HasColumnType("datetime");
            entity.Property(e => e.TxDetail)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ZTest>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("zTest");

            entity.Property(e => e.DateTm)
                .HasColumnType("datetime")
                .HasColumnName("DateTM");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.Value).HasColumnType("money");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
