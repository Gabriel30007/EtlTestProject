using EtlTestProject.DataMapping;

var table = DataMappingService.GetDataTabletFromCSVFile("C:\\Users\\User\\Downloads\\sample-cab-data.csv");

DataMappingService.InsertDataIntoSQLServerUsingSQLBulkCopy(table);