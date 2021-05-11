using System.Collections.Generic;
using System.IO;

using OfficeOpenXml;

using SatellitesTravelling.SatellitesModels;

namespace SatellitesTravelling
{
    public class SatelliteRepository
    {
        public static List<Satellite> ImporterSatellites()
        {
            List<Satellite> satelliteList = new List<Satellite>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage(new FileInfo(@"D:\Classeur1.xlsx"));

            ExcelWorksheet workSheet = package.Workbook.Worksheets["Feuil1"];

            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                // J colonne dans la table
                int j = 2;

                int Id = i - 2;
                //i: ligne . j++ pour augmenter colonne à chaque fois 
                string Satellite = workSheet.Cells[i, j++].Value.ToString();

                double Latitude = (double)workSheet.Cells[i, j++].Value;
                double Longitude = (double)workSheet.Cells[i, j++].Value;
                double Alt = (double)workSheet.Cells[i, j++].Value;
                // creer satellite
                Satellite satelliteO = new Satellite()
                {
                    Id = Id,
                    Name = Satellite,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Altitude = Alt

                };
                satelliteList.Add(satelliteO);
            }
            return satelliteList;
        }
    }
}

