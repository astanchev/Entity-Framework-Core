namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using ExportDto;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context
                .Prisoners
                .Where(p => ids.Contains(p.Id))
                .Select(p => new PrisonerExportDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                        .Select(o => new OfficerExportDto
                        {
                            OfficerName = o.Officer.FullName,
                            Department = o.Officer.Department.Name
                        })
                        .OrderBy(o => o.OfficerName)
                        .ToList(),
                    TotalOfficerSalary = Math.Round(p.PrisonerOfficers
                        .Sum(o => o.Officer.Salary), 2)
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var prisonersByCellJson = JsonConvert.SerializeObject(prisoners, Formatting.Indented);

            return prisonersByCellJson;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonersToFind = prisonersNames.Split(',').ToList();

            var prisoners = context
                .Prisoners
                .Where(p => prisonersToFind.Contains(p.FullName))
                .Select(p => new PrisonerInboxExportDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails
                        .Select(m => new MessageExportDto
                        {
                            Description = Reverse(m.Description)
                        })
                        .ToList()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var attr = new XmlRootAttribute("Prisoners");
            var serializer = new XmlSerializer(typeof(List<PrisonerInboxExportDto>), attr);

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            serializer.Serialize(new StringWriter(sb), prisoners, namespaces);

            return sb.ToString().TrimEnd();
        }
        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}