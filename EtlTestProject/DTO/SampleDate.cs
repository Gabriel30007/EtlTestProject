using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlTestProject.DTO
{
    public class SampleDate
    {
        public DateTime Tpep_pickup_datetime  { get; set; }
        public DateTime Tpep_dropoff_datetime  { get; set; }
	    public int Passenger_count  { get; set; }
        public decimal Trip_distance  { get; set; }
        public string Store_and_fwd_flag  { get; set; }
        public int PULocationID  { get; set; }
        public int DOLocationID  { get; set; }
        public decimal Fare_amount  { get; set; }
        public decimal Tip_amount  { get; set; }

        public SampleDate(DateTime tpep_pickup_datetime, DateTime tpep_dropoff_datetime, int passenger_count, decimal trip_distance, string store_and_fwd_flag, int pULocationID, int dOLocationID, decimal fare_amount, decimal tip_amount)
        {
            Tpep_pickup_datetime = tpep_pickup_datetime;
            Tpep_dropoff_datetime = tpep_dropoff_datetime;
            Passenger_count = passenger_count;
            Trip_distance = trip_distance;
            Store_and_fwd_flag = store_and_fwd_flag;
            PULocationID = pULocationID;
            DOLocationID = dOLocationID;
            Fare_amount = fare_amount;
            Tip_amount = tip_amount;
        }
    }
}
