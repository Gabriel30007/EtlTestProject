CREATE NONCLUSTERED INDEX IX_PULocationId_Includes  
ON SampleData (PULocationID)  
INCLUDE (tip_amount, tpep_pickup_datetime, tpep_dropoff_datetime, trip_distance);  