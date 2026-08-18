using Database.SQL;
using Dto;
using Dto.BaseSettings;
using Dto.Dto;
using Dto.Enums;
using Infrastructure;
using Newtonsoft.Json;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository.Repo
{
    public class InventoryRepo
    {
        private static readonly HttpClient _client = new HttpClient();
        private decimal _attemptCount = 1.0M;
        private int failCounter = 0;

        public InventoryDetailsDto ToDetails(Inventory inventory, DbSet<Database.SQL.User> users, bool isPHPDisplay = false)
        {
            var dto = new InventoryDetailsDto(ToDto(inventory, isPHPDisplay));

            if (dto == null) return null;

            if (inventory.Inventory_Count.Any())
            {
                dto.InventoryCounts = inventory.Inventory_Count
                    .Where(ic => !ic.IsDeleted)
                    .ToList()
                    .Select(ic => new InventoryCountRepo().ToDto(ic))
                    .ToList();
            }

            if (users != null && users.Any())
            {
                var owner = users.FirstOrDefault(a => a.Id == (dto.OwnerId));
                if (owner != null)
                {
                    dto.OwnerName = $"{owner.Firstname} {owner.LastName}";
                }
            }

            return dto;
        }

        public InventoryDto ToDto(Inventory inventory, bool isPHPDisplay = false)
        {
            if (inventory == null) return null;

            return new InventoryDto
            {
                Id = inventory.Id,
                Image = inventory.Image,
                Name = inventory.Name,
                SetCode = inventory.SetCode,
                SetName = inventory.SetName,
                Collector = inventory.Collector,
                Language = inventory.Language,
                FoilType = inventory.FoilType,
                Rarity = inventory.Rarity,
                ManaboxId = inventory.ManaboxId,
                ScryfallId = inventory.ScryfallId,
                Price = inventory.Price,
                Misprint = inventory.Misprint,
                Tampered = inventory.Tampered,
                Condition = inventory.Condition,
                PurchaseCurrency = isPHPDisplay ? "PHP" : inventory.PurchaseCurrency,
                DateCreated = inventory.DateCreated,
                CreatedBy = inventory.CreatedBy,
                IsDeleted = inventory.IsDeleted,
                Color = inventory.Color,
                Description = inventory.Description,
                IllustratedBy = inventory.IllustratedBy,
                CardType = inventory.CardType,
                ManaCost = inventory.ManaCost,
                CollectionGroup = inventory.CollectionGroup,
                OwnerId = inventory.OwnerId,
                Category = inventory.Category,
                IsPhpDisplay = isPHPDisplay,

            };
        }

        public InventoryDisplayDto ToDisplayDto(Inventory inventory, bool isPHPDisplay = false)
        {
            if (inventory == null) return null;

            return new InventoryDisplayDto
            {
                Id = inventory.Id,
                Name = inventory.Name,
                FoilType = inventory.FoilType,
                Price = inventory.Price,
                Currency = !isPHPDisplay ? inventory.PurchaseCurrency : "PHP",
                ImageBase64 = inventory.Image != null ? Convert.ToBase64String(inventory.Image) : string.Empty,
                MimeType = inventory.Image != null ? inventory.Image.GetImageExtension().Replace(".", "") : "png",
                Stock = (int)inventory.Inventory_Count.Sum(ic => ic.Quantity),
                IsPhpDisplay = isPHPDisplay,
            };
        }

        public InventoryDto GetById(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventories.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
                return ToDto(record);
            }
        }

        public InventoryDetailsDto GetById(string cardId, bool isPhpDisplay)
        {
            try
            {
                var id = int.Parse(Fletcher.Decrypt(cardId));

                using (IMSEntities context = new IMSEntities())
                {
                    var record = context.Inventories.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
                    return ToDetails(record, context.Users, isPhpDisplay);
                }
            }
            catch
            {
                return null;
            }
        }

        public InventoryDetailsDto GetDetailsById(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventories.Where(i => i.Id == id && !i.IsDeleted).Include(b => b.Inventory_Count);
                return ToDetails(record.FirstOrDefault(), context.Users);
            }
        }

        public IEnumerable<InventoryDetailsDto> GetList(
            string collectionGroup = "",
            int cardOwnerId = 0,
            string setCode = "",
            string category = "",
            string searchParam = "",
            bool isPHPDisplay = false,
            int userId = 0,
            string foilType = ""
        )
        {
            using (IMSEntities context = new IMSEntities())
            {
                var users = context.Users;
                var records = context.Inventories.Where(i => !i.IsDeleted).Include(b => b.Inventory_Count);

                var collectionGroups = collectionGroup.Split('|').Where(a => !string.IsNullOrEmpty(a));
                if (!collectionGroups.Any(a => a == "all"))
                    records = records.Where(i => collectionGroups.Any(b => i.CollectionGroup.ToLower() == b.ToLower()));

                if (userId > 0)
                {
                    records = records.Where(i => i.OwnerId == userId);
                }

                if (cardOwnerId > 0)
                {
                    records = records.Where(i => i.OwnerId == cardOwnerId);
                }

                if (!string.IsNullOrEmpty(setCode))
                {
                    records = records.Where(i => i.SetCode.ToLower() == setCode.ToLower());
                }

                var categories = category.Split('|').Where(a => !string.IsNullOrEmpty(a));
                if (categories.Any())
                {
                    records = records.Where(i => categories.Any(b => i.Category.ToLower() == b.ToLower()));
                }

                var foilTypes = foilType.Split('|').Where(a => !string.IsNullOrEmpty(a));
                if (foilTypes.Any())
                {
                    records = records.Where(i => foilTypes.Any(b => i.FoilType.ToLower() == b.ToLower()));
                }

                if (!string.IsNullOrEmpty(searchParam))
                {
                    records = records.Where(i => i.Name.ToLower().Contains(searchParam.ToLower()));
                }

                return records.ToList().Select(r => ToDetails(r, users, isPHPDisplay)).ToList();
            }
        }

        public Tuple<List<string>, List<string>, List<Tuple<int, string>>> GetFilters()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var records = context.Inventories.Where(i => !i.IsDeleted);

                var rarities = records.Select(i => i.Rarity).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                var foilTypes = records.Select(i => i.FoilType).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                var cardTypes = records.Select(i => i.CardType).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                var setCodes = records.Select(i => i.SetCode).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                var categories = records.Select(i => i.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                var cardOwnerIds = context.Users.Where(a => !a.IsDeleted & a.IsCardOwner).ToList().Select(i => Tuple.Create(i.Id, $"{i.Firstname} {i.LastName}")).Distinct().ToList();

                return Tuple.Create(setCodes, categories, cardOwnerIds);
            }
        }

        public IEnumerable<InventoryDisplayDto> GetListRandom(int count = 10)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var skip = new Random().Next(0, Math.Max(0, context.Inventories.Count(i => !i.IsDeleted) - count));

                var users = context.Users;
                var records = context.Inventories
                    .Where(i => !i.IsDeleted)
                    .OrderBy(a => a.Id)
                    .Skip(skip)
                    .Take(count)
                    .Include(b => b.Inventory_Count).ToList();
                return records.Select(r => ToDisplayDto(r, true)).ToList();
            }
        }

        public IEnumerable<CardDetailsDto> GetSearchResult(string searchParam = "", string colors = "",
            string rarities = "", string foilTypes = "", string cardTypes = "", string setName = "", string collection = "", int take = -1)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var records = context.Inventories.Where(i => !i.IsDeleted);

                if (!string.IsNullOrEmpty(collection))
                {
                    if (collection != "all")
                        records = records.Where(i => i.CollectionGroup.ToLower().Replace(" ", "") == collection.ToLower().Replace(" ", ""));
                }

                if (!string.IsNullOrEmpty(searchParam))
                {
                    records = records.Where(i => i.Name.ToLower().Contains(searchParam.ToLower()) || i.SetName.ToLower().Contains(searchParam.ToLower()) || i.SetCode.ToLower().Contains(searchParam.ToLower()));
                }

                if (!string.IsNullOrEmpty(colors) & collection == "magic the gathering")
                {
                    var colorList = colors.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => colorList.Any(c => i.Color.ToLower().Contains(c.ToLower())));
                }

                if (!string.IsNullOrEmpty(rarities) & collection == "magic the gathering")
                {
                    var rarityList = rarities.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => rarityList.Any(r => i.Rarity.ToLower().Replace(" ", "") == r.ToLower().Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(foilTypes))
                {
                    var foilTypeList = foilTypes.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => foilTypeList.Any(f => i.FoilType.ToLower().Replace(" ", "") == f.ToLower().Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(cardTypes))
                {
                    var cardTypeList = cardTypes.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => cardTypeList.Any(f => i.CardType.ToLower().Replace(" ", "") == f.ToLower().Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(setName))
                {
                    records = records.Where(i => i.SetName.ToLower().Contains(setName.ToLower()) || i.SetCode.ToLower().Contains(setName.ToLower()));
                }

                if (take > 0)
                {
                    records = records.Take(take);
                }

                var type = (int)InventoryCountTypeEnum.Sell;
                return records.Include(a => a.Inventory_Count).Select(r => new CardDetailsDto()
                {
                    Id = r.Id,
                    Image = r.Image,
                    Name = r.Name,
                    Price = r.Price,
                    CardType = r.CardType,
                    Count = r.Inventory_Count.Where(ic => !ic.IsDeleted).Sum(ic => (ic.Type == type ? -(ic.Quantity) : ic.Quantity)),
                    Rarity = r.Rarity,
                    FoilType = r.FoilType,
                    PurchaseCurrency = "PHP" //r.PurchaseCurrency,
                }).ToList();
            }
        }

        public ReturnValue Create(InventoryDetailsDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var inventory = new Inventory
                {
                    Image = dto.Image,
                    Name = dto.Name,
                    SetCode = dto.SetCode,
                    SetName = dto.SetName,
                    Collector = dto.Collector,
                    Language = dto.Language,
                    FoilType = dto.FoilType,
                    Rarity = dto.Rarity,
                    Condition = dto.Condition,
                    CreatedBy = dto.CreatedBy,
                    DateCreated = DateTime.Now,
                    IsDeleted = dto.IsDeleted,
                    ManaboxId = dto.ManaboxId,
                    Misprint = dto.Misprint,
                    PurchaseCurrency = dto.PurchaseCurrency,
                    Price = dto.Price,
                    ScryfallId = dto.ScryfallId,
                    Tampered = dto.Tampered,
                    Color = dto.Color,
                    Description = dto.Description,
                    CardType = dto.CardType,
                    IllustratedBy = dto.IllustratedBy,
                    ManaCost = dto.ManaCost,
                    CollectionGroup = dto.CollectionGroup,
                    OwnerId = dto.OwnerId,
                    Category = dto.Category,
                };

                context.Inventories.Add(inventory);

                Db.SaveChanges(context, result, "Inventory created successfully.");

                if (result.Success)
                {
                    if (!Directory.Exists(StoragePath.CardImageStoragePath))
                    {
                        Directory.CreateDirectory(StoragePath.CardImageStoragePath);
                    }

                    var path = Path.Combine(StoragePath.CardImageStoragePath, $"{inventory.Id.ToString()}.png");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.WriteAllBytes(path, inventory.Image);
                }
            }

            return result;
        }

        public ReturnValue CreateBulk(IEnumerable<InventoryDetailsDto> dtos)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var conversionRate = context.Conversions.FirstOrDefault(a => a.IsActive)?.Amount ?? 0;
                foreach (var dto in dtos)
                {
                    _attemptCount = _attemptCount + 0.1M;

                    switch (dto.CollectionGroup.ToLower().Replace(" ", ""))
                    {
                        case "grandarchive":
                            FetchClassDetails_GA(context, dto, conversionRate);
                            break;
                        case "magicthegathering":
                            FetchClassDetails_MTG(context, dto, conversionRate);
                            break;
                        default:
                            var inventory = new Inventory
                            {
                                Image = dto.Image ?? new byte[0],
                                Name = dto.Name,
                                SetCode = dto.SetCode,
                                SetName = dto.SetName,
                                Collector = dto.Collector,
                                Language = dto.Language,
                                FoilType = dto.FoilType,
                                Rarity = dto.Rarity,
                                Condition = dto.Condition,
                                CreatedBy = dto.CreatedBy,
                                DateCreated = DateTime.Now,
                                IsDeleted = dto.IsDeleted,
                                ManaboxId = dto.ManaboxId,
                                Misprint = dto.Misprint,
                                PurchaseCurrency = dto.PurchaseCurrency,
                                Price = dto.Price,
                                ScryfallId = dto.ScryfallId ?? "",
                                Tampered = dto.Tampered,
                                Color = dto.Color,
                                Description = dto.Description,
                                CardType = dto.CardType ?? "",
                                IllustratedBy = dto.IllustratedBy ?? "",
                                ManaCost = dto.ManaCost ?? "",
                                CollectionGroup = dto.CollectionGroup ?? "",
                                OwnerId = dto.OwnerId,
                                Category = dto.Category,
                            };

                            context.Inventories.Add(inventory);

                            if (dto.InventoryCounts.Any())
                            {
                                foreach (var countDto in dto.InventoryCounts)
                                {
                                    var newRecord = new Inventory_Count
                                    {
                                        DateCreated = countDto.DateCreated,
                                        CreatedBy = dto.CreatedBy,
                                        IsDeleted = countDto.IsDeleted,
                                        Quantity = countDto.Quantity,
                                        Remarks = dto.CreatedBy,
                                        Type = (int)countDto.Type,
                                        UOM = countDto.UOM
                                    };

                                    inventory.Inventory_Count.Add(newRecord);
                                }
                            }

                            try
                            {
                                var saveChanges = context.SaveChanges();
                            }
                            catch { failCounter++; }

                            break;
                    }
                }

                if (failCounter > 0)
                {
                    result.Success = false;
                    result.Message = "Bulk inventory has completed with some error.";
                }
                else
                {
                    result.Success = true;
                    result.Message = "Bulk inventory created successfully.";
                }
                //Db.SaveChanges(context, result, "Bulk inventory created successfully.");
            }

            return result;
        }

        public ReturnValue Update(InventoryDetailsDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var inventory = context.Inventories.FirstOrDefault(i => i.Id == dto.Id && !i.IsDeleted);

                if (inventory == null)
                    return new ReturnValue("Unable to find inventory item.");

                if (dto.Image.Length > 0)
                    inventory.Image = dto.Image;

                inventory.Name = dto.Name;
                inventory.SetCode = dto.SetCode;
                inventory.SetName = dto.SetName;
                inventory.Collector = dto.Collector;
                inventory.Language = dto.Language;
                inventory.FoilType = dto.FoilType;
                inventory.Rarity = dto.Rarity;
                inventory.Condition = dto.Condition;
                inventory.ManaboxId = dto.ManaboxId;
                inventory.Misprint = dto.Misprint;
                inventory.PurchaseCurrency = dto.PurchaseCurrency;
                inventory.Price = dto.Price;
                inventory.ScryfallId = dto.ScryfallId;
                inventory.Tampered = dto.Tampered;
                inventory.Color = dto.Color;
                inventory.Description = dto.Description;
                inventory.CardType = dto.CardType;
                inventory.IllustratedBy = dto.IllustratedBy;
                inventory.ManaCost = dto.ManaCost;
                inventory.OwnerId = dto.OwnerId;
                inventory.CollectionGroup = dto.CollectionGroup;
                inventory.Category = dto.Category;

                Db.SaveChanges(context, result, "Inventory updated successfully.");
                result.Data = ToDetails(inventory, context.Users);

                if (result.Success)
                {
                    if (!Directory.Exists(StoragePath.CardImageStoragePath))
                    {
                        Directory.CreateDirectory(StoragePath.CardImageStoragePath);
                    }

                    var path = Path.Combine(StoragePath.CardImageStoragePath, $"{inventory.Id.ToString()}.png");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.WriteAllBytes(path, inventory.Image);
                }

            }

            return result;
        }

        public static void UpdatePrice(int id, decimal amount)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var inventory = context.Inventories.FirstOrDefault(i => i.Id == id);

                if (inventory != null)
                {
                    inventory.Price = amount;
                    Db.SaveChanges(context);
                }
            }
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var inventory = context.Inventories.FirstOrDefault(i => i.Id == id && !i.IsDeleted);

                if (inventory == null)
                    return new ReturnValue("Unable to find inventory item.");

                inventory.IsDeleted = true;
                Db.SaveChanges(context, result, "Inventory deleted successfully.");
            }

            return result;
        }


        public ReturnValue BulkUpdate(string action, string ids, int ownerId)
        {
            var result = new ReturnValue("Unable to continue with your action");
            using (var context = new IMSEntities())
            {
                string message = "";
                var dataList = ids.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                switch (action)
                {
                    case "delete":
                        foreach (var id in dataList.Select(a => int.Parse(a)))
                        {
                            var inventory = context.Inventories.FirstOrDefault(i => i.Id == id);
                            if (inventory != null)
                            {
                                inventory.IsDeleted = true;
                            }
                        }
                        message = "Selected inventory items have been deleted.";
                        break;
                    case "move":
                        foreach (var id in dataList.Select(a => int.Parse(a)))
                        {
                            var inventory = context.Inventories.FirstOrDefault(i => i.Id == id);
                            if (inventory != null)
                            {
                                inventory.OwnerId = ownerId;
                            }
                        }
                        message = "Selected inventory items have been moved.";
                        break;
                    case "price":
                        foreach (var data in dataList)
                        {
                            var splitData = data.Split(':');
                            var id = int.Parse(splitData[0]);
                            var price = decimal.Parse(splitData[1]);

                            var inventory = context.Inventories.FirstOrDefault(i => i.Id == id);
                            if (inventory != null)
                            {
                                inventory.Price = price;
                            }
                        }
                        message = "Selected inventory item price have been updated.";
                        break;
                }

                Db.SaveChanges(context, result, message);
            }

            return result;
        }



        public void UpdatePricing(string collectionGroup = "")
        {
            using (IMSEntities context = new IMSEntities())
            {
                if (!Directory.Exists(StoragePath.CardImageStoragePath))
                {
                    Directory.CreateDirectory(StoragePath.CardImageStoragePath);
                }

                int index = 0;
                var conversionRate = context.Conversions.FirstOrDefault(a => a.IsActive)?.Amount ?? 1;
                var records = context.Inventories.Where(i => !i.IsDeleted);
                var collectionGroups = collectionGroup.Split('|').Where(a => !string.IsNullOrEmpty(a));

                Console.WriteLine($"Updating pricing for collection groups: {string.Join(", ", collectionGroups)} with conversion rate: {conversionRate}");

                records = records.Where(i => collectionGroups.Any(b => i.CollectionGroup.ToLower() == b.ToLower()));
                Console.WriteLine($"Total records before filtering: {records.Count()}");

                foreach (var record in records)
                {
                    var path = Path.Combine(StoragePath.CardImageStoragePath, $"{record.Id.ToString()}.png");

                    _attemptCount++;
                    index++;
                    Console.WriteLine($"{index}. {record.Name}");
                    switch (record.CollectionGroup.ToLower().Replace(" ", ""))
                    {
                        case "magicthegathering":
                            Console.WriteLine($"-> Getting record from Scryfall API");
                            var details_mtg = FetchCardDetailsAsync_Scryfall(record.ScryfallId, record.SetCode, record.Collector).Result;
                            if (details_mtg != null)
                            {

                                Console.WriteLine($"-> Updating.");
                                var isFoiled = record.FoilType.ToLower() != "non-foil" & record.FoilType.ToLower() != "normal";
                                if (isFoiled & details_mtg.Item2.Prices?.UsdFoil != null)
                                {
                                    record.Price = Convert.ToDecimal(details_mtg.Item2.Prices.UsdFoil) * conversionRate;
                                }
                                else if (details_mtg.Item2.Prices?.Usd != null)
                                {
                                    record.Price = Convert.ToDecimal(details_mtg.Item2.Prices.Usd) * conversionRate;
                                }
                                else if (details_mtg.Item2.Prices?.Usd == null & details_mtg.Item2.Prices?.UsdFoil == null & details_mtg.Item2.Prices?.UsdEtched != null)
                                {
                                    record.Price = Convert.ToDecimal(details_mtg.Item2.Prices.UsdEtched) * conversionRate;
                                }

                                if (details_mtg.Item1 != null && details_mtg.Item1.Length > 0)
                                {
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                    }

                                    File.WriteAllBytes(path, details_mtg.Item1);
                                }

                                Console.WriteLine($"-> Update for {record.Name} has been completed.");
                            }
                            break;
                        case "grandarchive":
                            Console.WriteLine($"-> Getting record from Grand Archive API");
                            record.ScryfallId = !string.IsNullOrEmpty(record.ScryfallId) ? record.ScryfallId : record.Name.Contains("-") ? Regex.Replace(record.Name, "-.*$", "").ToLower().Replace(" ", "-").Replace(",", "").Replace("'", "").Replace(":", "").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Replace("ß", "ss") : record.Name.ToLower().Replace(" ", "-").Replace(",", "").Replace("'", "").Replace(":", "").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Replace("ß", "ss");
                            var details_ga = FetchCardDetailsAsync_GrandArchive(record.ScryfallId, record.SetCode, record.Collector).Result;
                            if (details_ga != null)
                            {
                                Console.WriteLine($"-> Updating.");

                                record.Description = details_ga.Item2.effect ?? "";
                                record.ManaCost = $"{details_ga.Item2.cost.type}|{details_ga.Item2.cost.value}";
                                record.Color = string.Join(", ", details_ga.Item2.elements).TrimEnd();
                                record.CardType = string.Join(", ", details_ga.Item2.classes).TrimEnd();
                                record.IllustratedBy = details_ga.Item2.editions.FirstOrDefault()?.illustrator ?? "";
                                record.SetCode = !string.IsNullOrEmpty(record.SetCode) ? record.SetCode : (details_ga.Item2.editions.FirstOrDefault()?.set.prefix ?? "");
                                record.SetName = !string.IsNullOrEmpty(record.SetName) ? record.SetName : (details_ga.Item2.editions.FirstOrDefault()?.set.name ?? "");
                                record.Collector = !string.IsNullOrEmpty(record.Collector) ? record.Collector : (details_ga.Item2.editions.FirstOrDefault()?.collector_number ?? "");
                                record.Rarity = !string.IsNullOrEmpty(record.Rarity) ? record.Rarity : (details_ga.Item2.editions.FirstOrDefault()?.rarity.ToString() ?? "");
                                record.FoilType = !string.IsNullOrEmpty(record.FoilType) ? record.FoilType : "normal";
                                record.Language = !string.IsNullOrEmpty(record.Language) ? record.Language : (details_ga.Item2.editions.FirstOrDefault()?.set.language.ToLower() ?? "");

                                record.Image = details_ga.Item1;

                                if (details_ga.Item1 != null && details_ga.Item1.Length > 0)
                                {
                                    if (File.Exists(path))
                                    {
                                        File.Delete(path);
                                    }

                                    File.WriteAllBytes(path, details_ga.Item1);
                                }

                                Console.WriteLine($"-> Update for {record.Name} has been completed.");
                            }
                            break;
                        default: break;
                    }
                }

                Console.WriteLine($"Pricing update completed for {index}/{records.Count()} records. Saving changes to database...");
                var result = new ReturnValue();
                Db.SaveChanges(context, result, "Pricing update completed.");
                Console.WriteLine(result.Success ? "Database updated successfully." : $"Database update failed: {result.Message}");
            }
        }

        public async Task<Tuple<byte[], ScryfallCard>> FetchCardDetailsAsync_Scryfall(string scryfallId, string setCode, string collectorNumber)
        {
            var imageBytes = new byte[0];
            var card = new ScryfallCard();

            string cardUrl = $"https://api.scryfall.com/cards/{scryfallId}";

            if (!string.IsNullOrEmpty(scryfallId))
            {
                cardUrl = $"https://api.scryfall.com/cards/{scryfallId}";
            }
            else if (!string.IsNullOrEmpty(setCode) & !string.IsNullOrEmpty(collectorNumber))
            {
                cardUrl = $"https://api.gatcg.com/cards/{setCode}/{collectorNumber}";
            }

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", $"MyMTGApp{_attemptCount:n0}/{_attemptCount} (example@email.com)");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = _client.GetAsync(cardUrl).Result;
            response.EnsureSuccessStatusCode();

            var cardJson = response.Content.ReadAsStringAsync().Result;
            card = JsonConvert.DeserializeObject<ScryfallCard>(cardJson);


            if (card.CardFaces != null & card.CardFaces.Any())
            {
                //var cardFace = card.CardFaces.Where(a => a.Name == cardfaceName).FirstOrDefault();
                var cardFace = card.CardFaces.FirstOrDefault();

                if (cardFace != null)
                {
                    if (!string.IsNullOrEmpty(cardFace.ImageUris.Png))
                        imageBytes = _client.GetByteArrayAsync(cardFace.ImageUris.Png).Result;

                    //card.OracleText = cardFace?.OracleText ?? card.OracleText;
                    //card.Artist = cardFace?.Artist ?? card.Artist;
                    //card.ManaCost = cardFace?.ManaCost ?? card.ManaCost;
                    //card.TypeLine = cardFace?.TypeLine ?? card.TypeLine;
                    //card.ColorIdentity = cardFace?.Colors ?? card.ColorIdentity;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(card.ImageUris.Png))
                    imageBytes = _client.GetByteArrayAsync(card.ImageUris.Png).Result;
            }

            //_client.Dispose();
            return Tuple.Create(imageBytes, card);
        }

        public async Task<Tuple<byte[], GrandArchiveCard>> FetchCardDetailsAsync_GrandArchive(string slugName, string setCode, string collectorNumber)
        {
            var imageBytes = new byte[0];
            var card = new GrandArchiveCard();
            string cardUrl = $"https://api.gatcg.com/cards/{slugName}";

            if (!string.IsNullOrEmpty(setCode) & !string.IsNullOrEmpty(collectorNumber))
            {
                var isAlter = setCode.ToLower().Contains("alter");
                var setCodeTrimmed = isAlter ? setCode.ToLower().Replace(" alter", "") : setCode;
                setCodeTrimmed = setCodeTrimmed.Contains("1E") ? setCodeTrimmed.Replace("1E", "") : setCodeTrimmed;
                setCodeTrimmed = isAlter ? setCodeTrimmed + " Alter" : setCodeTrimmed;

                cardUrl = $"https://api.gatcg.com/cards/{setCodeTrimmed}/{collectorNumber}";
            }

            var cardJson = _client.GetStringAsync(cardUrl).Result;
            card = JsonConvert.DeserializeObject<GrandArchiveCard>(cardJson);

            if (card.editions != null & card.editions.Any())
            {
                var cardFace = card.editions.FirstOrDefault();

                if (cardFace != null)
                {
                    imageBytes = _client.GetByteArrayAsync($"https://api.gatcg.com{cardFace.image}").Result;
                }
            }

            //_client.Dispose();
            return Tuple.Create(imageBytes, card);
        }

        public string GetColorIdentityString(List<string> colorIdentity)
        {
            return string.Join("", colorIdentity.Select(a => "{" + a + "}"));
        }

        private void FetchClassDetails_MTG(IMSEntities context, InventoryDetailsDto dto, decimal conversionRate)
        {
            string faceName = dto.Name;
            if (dto.Name.Contains("//"))
            {
                faceName = dto.Name.Split(new string[] { "//" }, StringSplitOptions.None)[1].Trim();
            }

            var existing = context.Inventories.FirstOrDefault(i => i.ScryfallId == dto.ScryfallId && !i.IsDeleted);

            if (!Directory.Exists(StoragePath.CardImageStoragePath))
            {
                Directory.CreateDirectory(StoragePath.CardImageStoragePath);
            }
            Tuple<byte[], ScryfallCard> cardDetails = null;

            try { cardDetails = FetchCardDetailsAsync_Scryfall(dto.ScryfallId, dto.SetCode, dto.Collector)?.Result ?? null; }
            catch { }
            if (existing != null)
            {
                if (cardDetails != null)
                {
                    if (string.IsNullOrEmpty(existing.Description))
                        existing.Description = cardDetails.Item2.OracleText ?? "";

                    if (string.IsNullOrEmpty(existing.Color))
                        dto.Color = GetColorIdentityString(cardDetails.Item2.ColorIdentity);

                    if (string.IsNullOrEmpty(existing.ManaCost))
                        existing.ManaCost = cardDetails.Item2.ManaCost;

                    if (string.IsNullOrEmpty(existing.CardType))
                        existing.CardType = cardDetails.Item2.TypeLine;

                    if (string.IsNullOrEmpty(existing.IllustratedBy))
                        existing.IllustratedBy = cardDetails.Item2.Artist;

                    var currentPrice = existing.Price / conversionRate;
                    var isFoiled = existing.FoilType.ToLower() != "non-foil" & existing.FoilType.ToLower() != "normal";
                    var scryfallPrice = Convert.ToDecimal(isFoiled ? (cardDetails.Item2.Prices?.UsdFoil ?? dto.Price.ToString()) : (cardDetails.Item2.Prices?.Usd ?? dto.Price.ToString()));
                    if (currentPrice != scryfallPrice)
                        existing.Price = scryfallPrice * conversionRate;
                }

                existing.Name = dto.Name;
                existing.SetCode = dto.SetCode;
                existing.SetName = dto.SetName;
                existing.Collector = dto.Collector;
                existing.Language = dto.Language;
                existing.FoilType = dto.FoilType;
                existing.Rarity = dto.Rarity;
                existing.ManaboxId = dto.ManaboxId;
                existing.ScryfallId = dto.ScryfallId;
                existing.Misprint = dto.Misprint;
                existing.Tampered = dto.Tampered;
                existing.Condition = dto.Condition;
                existing.PurchaseCurrency = dto.PurchaseCurrency;
                existing.Color = dto.Color;
                existing.ManaCost = dto.ManaCost;
                existing.CardType = dto.CardType;
                existing.IllustratedBy = dto.IllustratedBy;
                existing.CollectionGroup = dto.CollectionGroup;
                existing.OwnerId = dto.OwnerId;
                existing.Category = dto.Category;

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(existing, countDto);
                    }
                }
            }
            else
            {
                if (cardDetails != null)
                    dto.Color = GetColorIdentityString(cardDetails.Item2.ColorIdentity);

                existing = new Inventory
                {
                    Image = cardDetails?.Item1 ?? new byte[0],
                    Name = dto.Name,
                    SetCode = dto.SetCode,
                    SetName = dto.SetName,
                    Collector = dto.Collector,
                    Language = dto.Language,
                    FoilType = dto.FoilType,
                    Rarity = dto.Rarity,
                    ManaboxId = dto.ManaboxId,
                    ScryfallId = dto.ScryfallId,
                    Price = dto.Price,
                    Misprint = dto.Misprint,
                    Tampered = dto.Tampered,
                    Condition = dto.Condition,
                    PurchaseCurrency = dto.PurchaseCurrency,
                    DateCreated = dto.DateCreated,
                    CreatedBy = dto.CreatedBy,
                    IsDeleted = dto.IsDeleted,
                    Color = dto.Color,
                    Description = cardDetails?.Item2.OracleText ?? "",
                    CardType = cardDetails?.Item2.TypeLine ?? "",
                    IllustratedBy = cardDetails?.Item2.Artist ?? "",
                    ManaCost = cardDetails?.Item2.ManaCost ?? "",
                    OwnerId = dto.OwnerId,
                    CollectionGroup = dto.CollectionGroup,
                    Category = dto.Category,
                };

                if (cardDetails != null)
                {
                    var currentPrice = existing.Price / conversionRate;
                    var isFoiled = existing.FoilType.ToLower() != "non-foil" & existing.FoilType.ToLower() != "normal";
                    var scryfallPrice = Convert.ToDecimal(isFoiled ? (cardDetails.Item2.Prices?.UsdFoil ?? dto.Price.ToString()) : (cardDetails.Item2.Prices?.Usd ?? dto.Price.ToString()));
                    if (currentPrice != scryfallPrice)
                    {
                        existing.Price = scryfallPrice * conversionRate;
                        existing.PurchaseCurrency = "USD";
                    }
                }

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(existing, countDto);
                    }
                }

                context.Inventories.Add(existing);
            }

            try
            {
                var saveChanges = context.SaveChanges();

                if (cardDetails != null)
                {
                    if (saveChanges >= 0)
                    {
                        if (cardDetails.Item1 != null && cardDetails.Item1.Length > 0)
                        {
                            var path = Path.Combine(StoragePath.CardImageStoragePath, $"{existing.ToString()}.png");
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                            File.WriteAllBytes(path, cardDetails.Item1);
                        }
                    }
                }
            }
            catch { failCounter++; }
        }

        private void FetchClassDetails_GA(IMSEntities context, InventoryDetailsDto dto, decimal conversionRate)
        {
            var existing = context.Inventories.FirstOrDefault(i => i.ScryfallId == dto.ScryfallId && !i.IsDeleted);
            var cardNameAfterSlash = dto.Name.Contains("-") ? Regex.Replace(dto.Name, "-.*$", "") : dto.Name;
            var slugName = cardNameAfterSlash.ToLower().Replace(" ", "-").Replace(",", "").Replace("'", "").Replace(":", "").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Replace("ß", "ss");
            Tuple<byte[], GrandArchiveCard> cardDetails = FetchCardDetailsAsync_GrandArchive(slugName, dto.SetCode, dto.Collector).Result;

            if (existing != null)
            {
                if (existing.ScryfallId != slugName)
                {
                    existing.ScryfallId = slugName;
                }

                if (existing.Image == null || existing.Image.Length == 0)
                    existing.Image = cardDetails.Item1;

                if (string.IsNullOrEmpty(existing.Description))
                    existing.Description = cardDetails.Item2.effect ?? "";

                if (string.IsNullOrEmpty(existing.ManaCost))
                    existing.ManaCost = $"{cardDetails.Item2.cost.type}|{cardDetails.Item2.cost.value}";

                if (string.IsNullOrEmpty(existing.Color))
                    existing.Color = string.Join(", ", cardDetails.Item2.elements).TrimEnd();

                if (string.IsNullOrEmpty(existing.CardType))
                    existing.CardType = string.Join(", ", cardDetails.Item2.classes).TrimEnd();

                if (string.IsNullOrEmpty(existing.IllustratedBy))
                    existing.IllustratedBy = cardDetails.Item2.editions.FirstOrDefault()?.illustrator ?? "";

                existing.Name = dto.Name;
                existing.SetCode = !string.IsNullOrEmpty(dto.SetCode) ? dto.SetCode : (cardDetails.Item2.editions.FirstOrDefault()?.set.prefix ?? "");
                existing.SetName = !string.IsNullOrEmpty(dto.SetName) ? dto.SetName : (cardDetails.Item2.editions.FirstOrDefault()?.set.name ?? "");
                existing.Collector = !string.IsNullOrEmpty(dto.Collector) ? dto.Collector : (cardDetails.Item2.editions.FirstOrDefault()?.collector_number ?? "");
                existing.Language = dto.Language;
                existing.FoilType = dto.FoilType;
                existing.Rarity = !string.IsNullOrEmpty(dto.Rarity) ? dto.Rarity : (cardDetails.Item2.editions.FirstOrDefault()?.rarity.ToString() ?? "");
                existing.ManaboxId = dto.ManaboxId;
                existing.Misprint = dto.Misprint;
                existing.Tampered = dto.Tampered;
                existing.Condition = dto.Condition;
                existing.PurchaseCurrency = dto.PurchaseCurrency;
                existing.CollectionGroup = dto.CollectionGroup;
                existing.OwnerId = dto.OwnerId;
                existing.Category = dto.Category;
                existing.Price = dto.Price;

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(existing, countDto);
                    }
                }
            }
            else
            {
                existing = new Inventory
                {
                    Name = dto.Name,
                    SetCode = dto.SetCode ?? "",
                    SetName = dto.SetName ?? "",
                    Collector = dto.Collector ?? "",
                    Language = dto.Language,
                    FoilType = dto.FoilType,
                    Rarity = dto.Rarity ?? "",
                    ManaboxId = dto.ManaboxId,
                    Price = dto.Price,
                    Misprint = dto.Misprint,
                    Tampered = dto.Tampered,
                    Condition = dto.Condition,
                    PurchaseCurrency = dto.PurchaseCurrency,
                    DateCreated = dto.DateCreated,
                    CreatedBy = dto.CreatedBy,
                    IsDeleted = dto.IsDeleted,
                    Color = dto.Color ?? "",
                    OwnerId = dto.OwnerId,
                    CollectionGroup = dto.CollectionGroup,
                    Category = dto.Category,
                };

                existing.ScryfallId = slugName;
                existing.Image = cardDetails?.Item1 ?? new byte[0];
                existing.Description = cardDetails.Item2.effect ?? "";
                existing.ManaCost = $"{cardDetails.Item2.cost.type}|{cardDetails.Item2.cost.value}";
                existing.Color = string.Join(", ", cardDetails.Item2.elements).TrimEnd();
                existing.CardType = string.Join(", ", cardDetails.Item2.classes).TrimEnd();
                existing.IllustratedBy = cardDetails.Item2.editions.FirstOrDefault()?.illustrator ?? "";
                existing.SetCode = !string.IsNullOrEmpty(dto.SetCode) ? dto.SetCode : (cardDetails.Item2.editions.FirstOrDefault()?.set.prefix ?? "");
                existing.SetName = !string.IsNullOrEmpty(dto.SetName) ? dto.SetName : (cardDetails.Item2.editions.FirstOrDefault()?.set.name ?? "");
                existing.Collector = !string.IsNullOrEmpty(dto.Collector) ? dto.Collector : (cardDetails.Item2.editions.FirstOrDefault()?.collector_number ?? "");
                existing.Rarity = !string.IsNullOrEmpty(dto.Rarity) ? dto.Rarity : (cardDetails.Item2.editions.FirstOrDefault()?.rarity.ToString() ?? "");


                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(existing, countDto);
                    }
                }

                context.Inventories.Add(existing);
            }

            try
            {
                var saveChanges = context.SaveChanges();

                if (saveChanges >= 0)
                {
                    if (cardDetails.Item1 != null && cardDetails.Item1.Length > 0)
                    {
                        var path = Path.Combine(StoragePath.CardImageStoragePath, $"{existing.ToString()}.png");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        File.WriteAllBytes(path, cardDetails.Item1);
                    }
                }
            }
            catch { failCounter++; }

        }

    }
}
