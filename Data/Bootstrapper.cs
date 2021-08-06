using System.Collections.Generic;
using System.Linq;
using SUShirts.Data.Entities;
using SUShirts.Data.Enums;

namespace SUShirts.Data
{
    public static class Bootstrapper
    {
        public static void BootstrapDatabase(AppDbContext dbContext)
        {
            if (dbContext.Shirts.Any())
            {
                return;
            }

            var colorNames = new[]
            {
                "Světle modrá", "Tmavě modrá", "Oranžová", "Tmavě oranžová", "Šedá", "Zelená", "Bílá", "Černá", "Žlutá",
                "Tmavě zelená", "Červená", "Fialová", "Vícebarevné"
            };

            var colorHexes = new[]
            {
                "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000", "000000",
                "000000", "000000", "000000", "000000"
            };

            var colors = new List<Color>();
            for (var i = 0; i < colorNames.Length; i++)
            {
                colors.Add(new Color() {Name = colorNames[i], Hex = colorHexes[i]});
            }

            var shirts = new[]
            {
                new Shirt()
                {
                    Name = "FIT oranžové",
                    PrimaryColor = colors[2],
                    SecondaryColor = colors[7],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.M,
                            ItemsLeft = 2
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.L,
                            ItemsLeft = 3
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XL,
                            ItemsLeft = 3
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 2
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT šedé",
                    PrimaryColor = colors[4],
                    SecondaryColor = colors[6],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.S,
                            ItemsLeft = 1
                        },
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.M,
                            ItemsLeft = 2
                        },
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.L,
                            ItemsLeft = 6
                        },
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.XL,
                            ItemsLeft = 4
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT šedé (zelený text)",
                    PrimaryColor = colors[4],
                    SecondaryColor = colors[5],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.L,
                            ItemsLeft = 2
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT bílé",
                    PrimaryColor = colors[6],
                    SecondaryColor = colors[7],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Unisex,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 3
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT modré",
                    PrimaryColor = colors[0],
                    SecondaryColor = colors[7],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 8
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.M,
                            ItemsLeft = 9
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.L,
                            ItemsLeft = 10
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 6
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT žluté",
                    PrimaryColor = colors[8],
                    SecondaryColor = colors[7],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.M,
                            ItemsLeft = 7
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.L,
                            ItemsLeft = 6
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XL,
                            ItemsLeft = 9
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 3
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT tmavě zelené",
                    PrimaryColor = colors[9],
                    SecondaryColor = colors[6],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Unisex,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 9
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT červené (s bílým textem)",
                    PrimaryColor = colors[10],
                    SecondaryColor = colors[6],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 1
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.L,
                            ItemsLeft = 5
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XL,
                            ItemsLeft = 5
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 6
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT fialové",
                    PrimaryColor = colors[11],
                    SecondaryColor = colors[6],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.M,
                            ItemsLeft = 4
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.L,
                            ItemsLeft = 7
                        },
                        new()
                        {
                            Sex = SexVariant.Woman,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 2
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT červené (s černým textem)",
                    PrimaryColor = colors[10],
                    SecondaryColor = colors[7],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.S,
                            ItemsLeft = 4
                        },
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.M,
                            ItemsLeft = 7
                        },
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.L,
                            ItemsLeft = 3
                        },
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.XL,
                            ItemsLeft = 5
                        }
                    }
                },
                new Shirt()
                {
                    Name = "FIT tmavě oranžové",
                    PrimaryColor = colors[3],
                    SecondaryColor = colors[6],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Man,
                            Size = SizeVariant.XXL,
                            ItemsLeft = 8
                        }
                    }
                },
                new Shirt()
                {
                    Name = "Kachna",
                    PrimaryColor = colors[1],
                    SecondaryColor = colors[12],
                    Variants = new List<ShirtVariant>()
                    {
                        new()
                        {
                            Sex = SexVariant.Unisex,
                            Size = SizeVariant.M,
                            ItemsLeft = 4
                        },
                        new()
                        {
                            Sex = SexVariant.Unisex,
                            Size = SizeVariant.L,
                            ItemsLeft = 8
                        }
                    }
                },
            };

            dbContext.Colors.AddRange(colors);
            dbContext.Shirts.AddRange(shirts);
            dbContext.SaveChanges();
        }
    }
}
