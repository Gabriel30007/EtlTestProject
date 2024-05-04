CREATE TABLE SampleData
(	
	tpep_pickup_datetime datetime,
	tpep_dropoff_datetime datetime,
	passenger_count INT,
    trip_distance decimal,
    store_and_fwd_flag nvarchar(3),
    PULocationID INT,
    DOLocationID INT,
    fare_amount DECIMAL,
    tip_amount DECIMAL
)