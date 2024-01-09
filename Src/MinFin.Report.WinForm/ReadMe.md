# How to change target year of the report 

Replace the where clause in the designer of SSMS (as there are no SQL files):
  BETWEEN '2023-01-01' AND '2023-12-31 23:59:59'
for
  Vw_TaxLiqReport
  Vw_TaxLiqReport_Alx
  Vw_TaxLiqReport_Mei

 