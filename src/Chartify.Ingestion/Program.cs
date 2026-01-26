using Chartify.Ingestion.Scripts;


await using var csvStream = await DownloadCsv.RunAsync();


var chart = ParseCsv.Parse(csvStream);

await ImportCsv.RunAsync(chart);
