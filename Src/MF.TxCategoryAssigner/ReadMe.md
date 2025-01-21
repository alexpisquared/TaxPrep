Reverse CHrono

# 2025-01-20  Publish:

MinFin4 - ClickOnce does not work .. use Rls build as an installed/published app.
MinFin7 - Publish to default folder: c:\g\taxprep\src\minfin7\bin\release\net9.0-windows8.0\publish\

# 2021-09-02  Still .Net 4.8 'cause of .. TTS ... maybe
ClickOnce does not work .. use Rls folder as an installed/published app.

//todo: read balances in from "C:\Users\alexp\OneDrive\Documents\0\MF\DnLds\accountactivity TD Pi 2020-030-02  -  2021-09-02.csv" to txnV2 table.


# 2021-01-17
  Renamed/retired Vw_Exp_Hist_vs_2019 to Vw_Exp_Hist_vs_Last.

 ## Overall process of bringing the code to the next tax year:
  1. update trg year in C:\g\TaxPrep\Src\MF.TxCategoryAssigner\sql\Vw_Exp_Hist_vs_Last.sql
  2. update trg year in C:\g\TaxPrep\Src\MF.TxCategoryAssigner\sql\Vw_TaxLiqReport['', '_Alx', '_Mei'].sql
  3. 
