namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data.Models;
    using Data.Models.Enums;
    using ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentsDto = JsonConvert.DeserializeObject<List<DepartmentImportDto>>(jsonString);

            StringBuilder sb = new StringBuilder();
            var validDepartments = new List<Department>();

            foreach (var dto in departmentsDto)
            {
                if (!IsValid(dto) || !dto.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var department = new Department
                {
                    Id = 0,
                    Name = dto.Name,
                    Cells = dto.Cells
                        .Select(c => new Cell
                        {
                            CellNumber = c.CellNumber,
                            HasWindow = c.HasWindow
                        })
                        .ToList()
                };
                
                validDepartments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonersDto = JsonConvert.DeserializeObject<List<PrisonerImportDto>>(jsonString);

            StringBuilder sb = new StringBuilder();
            var validPrisoners = new List<Prisoner>();

            foreach (var dto in prisonersDto)
            {
                if (!IsValid(dto) || !dto.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var prisoner = new Prisoner
                {
                    FullName = dto.FullName,
                    Nickname = dto.Nickname,
                    Age = dto.Age,
                    IncarcerationDate = DateTime.ParseExact(dto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReleaseDate = dto.ReleaseDate == null 
                                  ? (DateTime?) null
                                  : DateTime.ParseExact(dto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Bail = dto.Bail,
                    CellId = dto.CellId,
                    Mails = dto.Mails
                        .Select(m => new Mail
                        {
                            Description = m.Description,
                            Sender = m.Sender,
                            Address = m.Address
                        })
                        .ToList()
                };
                
                validPrisoners.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var attr = new XmlRootAttribute("Officers");
            var serializer = new XmlSerializer(typeof(List<OfficerImportDto>), attr);

            StringBuilder sb = new StringBuilder();
            var validOfficers = new List<Officer>();

            using (StringReader reader = new StringReader(xmlString))
            {
                var officersDto = (List<OfficerImportDto>)serializer.Deserialize(reader);

                foreach (var dto in officersDto)
                {
                    if (!IsValid(dto) 
                        || !dto.Prisoners.All(IsValid))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    var resultPosition = Enum.TryParse(dto.Position, out Position positionType);
                    var resultWeapon = Enum.TryParse(dto.Weapon, out Weapon weaponType);

                    if (!resultPosition || !resultWeapon)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    
                    var officer = new Officer
                    {
                        FullName = dto.Name,
                        Salary = dto.Money,
                        Position = positionType,
                        Weapon = weaponType,
                        DepartmentId = dto.DepartmentId
                    };

                    foreach (var prisoner in dto.Prisoners)
                    {
                        var validPrisoner = new Prisoner
                        {
                            Id = prisoner.Id
                        };

                        var officerPrisoner = new OfficerPrisoner
                        {
                            Prisoner = validPrisoner,
                            Officer = officer
                        };

                        officer.OfficerPrisoners.Add(officerPrisoner);
                    }

                    validOfficers.Add(officer);

                    sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            var result = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return result;
        }
    }
}