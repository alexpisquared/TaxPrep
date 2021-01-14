using AsLink;
using Db.FinDemo.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.FinDemo.PartExt
{
	public class DBInitializer : DropCreateDatabaseIfModelChanges<A0DbContext>
	{
		protected override void Seed(A0DbContext ctx)
		{
			try
			{
				base.Seed(ctx);

				var now = DateTime.Now;

				ctx.ExpenseGroups.Add(new ExpenseGroup { Id = "Auto", Name = "1. Automobile", Note = now.ToString() });
				ctx.ExpenseGroups.Add(new ExpenseGroup { Id = "Util", Name = "2. Utilities", Note = now.ToString() });
				ctx.ExpenseGroups.Add(new ExpenseGroup { Id = "Other", Name = "3. Other Business Expenses", Note = now.ToString() });
				ctx.ExpenseGroups.Add(new ExpenseGroup { Id = "NoBiz", Name = "4. Non-Business Deductions", Note = now.ToString() });
				ctx.ExpenseGroups.Add(new ExpenseGroup { Id = "UnKn", Name = "5. Unknown", Note = now.ToString() });
				ctx.ExpenseGroups.Add(new ExpenseGroup { Id = "Ignore", Name = "Ignore", Note = now.ToString() });

				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "--Unknown ", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "Cash _____", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "Debit12345", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "Debit45678", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "VISA*98765", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "MasterCard", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "AmEx!!!!!!", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "VISA*12345", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "VISA*45678", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "AmEx*54321", CreatedAt = now });
				ctx.TxMoneySrcs.Add(new TxMoneySrc { Name = "PC Debit*1", CreatedAt = now });

				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuDnP", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuFix", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuGas", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuIns", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuInt", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuLic", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuPrk", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Auto", Name = "123", IdTxt = "AuTol", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Util", Name = "123", IdTxt = "Gas", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Util", Name = "123", IdTxt = "HIns", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Util", Name = "123", IdTxt = "Hydro", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Util", Name = "123", IdTxt = "I-Net", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Util", Name = "123", IdTxt = "Water", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Util", Name = "123", IdTxt = "Phon", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "BankFee", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Clean", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Com", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "DonatnB", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "FrnElec", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Gift", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "LIns", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Meal", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "PC", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Post", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "ProFee", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Salary", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Tools", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Tran", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Travel", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Other", Name = "123", IdTxt = "Uniform", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "KidCare", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "KidFit", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "Med", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "PTax", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "PubTr", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "RSP", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "DonatnP", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "NoBiz", Name = "123", IdTxt = "FitClub", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Ignore", Name = "123", IdTxt = "Party", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Ignore", Name = "123", IdTxt = "Ignore", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Ignore", Name = "123", IdTxt = "Late", DeductiblePercentage = 1.0, CreatedAt = now });
				ctx.TxCategories.Add(new TxCategory { ExpGroupId = "Ignore", Name = "123", IdTxt = "UnKn", DeductiblePercentage = 1.0, CreatedAt = now });

				ctx.SaveChanges();
			}
			catch (DbUpdateException ex) { Trace.WriteLine(ex.DbUpdateExceptionToString()); throw; }
			catch (Exception ex) { Trace.WriteLine(ex.ToString()); throw; }
		}

		public static void DropCreateDB()
		{
			if (new string[] { "ASUS2", "OCDT70342422" }.Contains(Environment.MachineName)) return;

			try
			{
				Database.SetInitializer(new CreateDatabaseIfNotExists<A0DbContext>());
				//Database.SetInitializer(new DropCreateDatabaseIfModelChanges<A0DbContext>());
				Database.SetInitializer(new DBInitializer());

				A0DbContext.Create().TxMoneySrcs.Load(); // triggers the DB creation.
			}
			catch (NotSupportedException ex) { Trace.WriteLine("\nAP: IGNORE: " + ex.Message); }
			catch (Exception ex) { Trace.WriteLine("\n" + ex.ToString()); }
		}
	}
}
