using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCareApp.Services;

public static class CarCatalogService
{
    // Модель автомобиля с поколениями
    public class ModelInfo
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Generations { get; set; } = new();

        public ModelInfo(string name, params string[] generations)
        {
            Name = name;
            Generations = generations.ToList();
        }
    }

    // Иерархический каталог: Марка -> Модели -> Поколения
    private static readonly Dictionary<string, List<ModelInfo>> Catalog = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Lada (ВАЗ)"] = new()
        {
            new("Vesta", "I (2015–2022)", "I Рестайлинг NG (2022–н.в.)", "Cross", "SW Cross"),
            new("Granta", "I (2011–2018)", "I Рестайлинг (2018–н.в.)", "Cross"),
            new("Priora", "I (2007–2013)", "I Рестайлинг (2013–2018)"),
            new("Kalina", "I (2004–2013)", "II (2013–2018)", "Cross"),
            new("Niva Legend (4x4)", "2121 (1977–н.в.)", "Urban"),
            new("Niva Travel", "I (2020–н.в.)", "Chevrolet Niva (2002–2020)"),
            new("Largus", "I (2012–2021)", "I Рестайлинг (2021–н.в.)", "Cross"),
            new("2114 (Самара-2)", "I (2001–2013)"),
            new("2110 (Десятка)", "I (1995–2007)"),
            new("2107", "I (1982–2012)")
        },
        ["Hyundai"] = new()
        {
            new("Solaris", "I (2010–2014)", "I Рестайлинг (2014–2017)", "II (2017–2020)", "II Рестайлинг (2020–н.в.)"),
            new("Elantra", "IV (HD, 2006–2011)", "V (MD, 2010–2016)", "VI (AD, 2015–2020)", "VII (CN7, 2020–н.в.)"),
            new("Creta", "I (2016–2021)", "II (2021–н.в.)"),
            new("Tucson", "II (ix35, 2009–2015)", "III (TL, 2015–2021)", "IV (NX4, 2020–н.в.)"),
            new("Santa Fe", "III (DM, 2012–2018)", "IV (TM, 2018–2024)", "V (MX5, 2023–н.в.)"),
            new("Sonata", "VI (YF, 2009–2014)", "VII (LF, 2014–2019)", "VIII (DN8, 2019–н.в.)")
        },
        ["Kia"] = new()
        {
            new("Rio", "III (2011–2017)", "IV (2017–2020)", "IV Рестайлинг (2020–н.в.)", "X-Line"),
            new("Ceed", "I (ED, 2006–2012)", "II (JD, 2012–2018)", "III (CD, 2018–н.в.)"),
            new("Cerato", "II (TD, 2008–2013)", "III (YD, 2013–2018)", "IV (BD, 2018–н.в.)"),
            new("Sportage", "III (SL, 2010–2016)", "IV (QL, 2015–2022)", "V (NQ5, 2021–н.в.)"),
            new("K5 / Optima", "Optima IV (JF, 2015–2020)", "K5 (DL3, 2019–н.в.)"),
            new("Sorento", "II (XM, 2009–2021)", "III (Prime/UM, 2014–2020)", "IV (MQ4, 2020–н.в.)")
        },
        ["Toyota"] = new()
        {
            new("Camry", "XV40 (2006–2011)", "XV50 (2011–2017)", "XV70 (2017–2024)", "XV80 (2023–н.в.)"),
            new("Corolla", "IX (E120, 2000–2008)", "X (E140/E150, 2006–2013)", "XI (E160/E180, 2012–2019)", "XII (E210, 2018–н.в.)"),
            new("RAV4", "III (XA30, 2005–2016)", "IV (XA40, 2012–2019)", "V (XA50, 2018–н.в.)"),
            new("Land Cruiser Prado", "120 (2002–2009)", "150 (2009–2023)", "250 (2023–н.в.)"),
            new("Land Cruiser", "100 (1998–2007)", "200 (2007–2021)", "300 (2021–н.в.)"),
            new("Highlander", "II (U40, 2007–2014)", "III (U50, 2013–2020)", "IV (U70, 2019–н.в.)")
        },
        ["Volkswagen"] = new()
        {
            new("Polo", "V Седан (2010–2020)", "VI Лифтбек (2020–н.в.)"),
            new("Tiguan", "I (2007–2016)", "II (2016–2023)", "III (2023–н.в.)"),
            new("Passat", "B6 (2005–2010)", "B7 (2010–2015)", "B8 (2014–2023)"),
            new("Golf", "V (2003–2009)", "VI (2008–2013)", "VII (2012–2020)", "VIII (2019–н.в.)"),
            new("Touareg", "I (2002–2010)", "II (2010–2018)", "III (2018–н.в.)")
        },
        ["Skoda"] = new()
        {
            new("Octavia", "II (A5, 2004–2013)", "III (A7, 2013–2020)", "IV (A8, 2019–н.в.)"),
            new("Rapid", "I (2012–2020)", "II (2020–н.в.)"),
            new("Kodiaq", "I (2016–2023)", "II (2023–н.в.)"),
            new("Superb", "II (2008–2015)", "III (2015–2023)", "IV (2023–н.в.)")
        },
        ["Renault"] = new()
        {
            new("Logan", "I (2004–2015)", "II (2014–2022)", "Stepway"),
            new("Duster", "I (2010–2021)", "II (2021–н.в.)"),
            new("Sandero", "I (2009–2014)", "II (2014–2022)", "Stepway"),
            new("Kaptur", "I (2016–н.в.)"),
            new("Arkana", "I (2019–н.в.)")
        },
        ["BMW"] = new()
        {
            new("3 серия", "E90 (2005–2013)", "F30 (2011–2019)", "G20 (2018–н.в.)"),
            new("5 серия", "E60 (2003–2010)", "F10 (2010–2017)", "G30 (2016–2023)", "G60 (2023–н.в.)"),
            new("X5", "E53 (1999–2006)", "E70 (2006–2013)", "F15 (2013–2018)", "G05 (2018–н.в.)"),
            new("X3", "E83 (2003–2010)", "F25 (2010–2017)", "G01 (2017–2024)", "G45 (2024–н.в.)")
        },
        ["Mercedes-Benz"] = new()
        {
            new("C-Класс", "W204 (2007–2015)", "W205 (2014–2021)", "W206 (2021–н.в.)"),
            new("E-Класс", "W211 (2002–2009)", "W212 (2009–2016)", "W213 (2016–2023)", "W214 (2023–н.в.)"),
            new("S-Класс", "W221 (2005–2013)", "W222 (2013–2020)", "W223 (2020–н.в.)"),
            new("GLE / ML", "ML W164 (2005–2011)", "ML/GLE W166 (2011–2018)", "GLE V167 (2018–н.в.)")
        },
        ["Haval"] = new()
        {
            new("Jolion", "I (2020–2024)", "I Рестайлинг (2024–н.в.)"),
            new("F7", "I (2019–2024)", "II (2024–н.в.)"),
            new("F7x", "I (2019–2024)"),
            new("Dargo", "I (2020–н.в.)"),
            new("M6", "I (2017–н.в.)"),
            new("H9", "I (2014–2024)", "II (2024–н.в.)")
        },
        ["Geely"] = new()
        {
            new("Coolray", "I (2019–2023)", "I Рестайлинг (2023–н.в.)"),
            new("Monjaro", "I (2021–н.в.)"),
            new("Atlas Pro", "I (2021–н.в.)"),
            new("Atlas", "I (2016–2021)", "II (2023–н.в.)"),
            new("Tugella", "I (2019–н.в.)"),
            new("Emgrand", "II (2021–н.в.)")
        },
        ["Chery"] = new()
        {
            new("Tiggo 4 / 4 Pro", "I (2017–2022)", "4 Pro (2022–н.в.)"),
            new("Tiggo 7 Pro / Max", "7 Pro (2020–2023)", "7 Pro Max (2023–н.в.)"),
            new("Tiggo 8 Pro / Max", "8 Pro (2021–н.в.)", "8 Pro Max (2022–н.в.)"),
            new("Arrizo 8", "I (2022–н.в.)")
        },
        ["Changan"] = new()
        {
            new("CS35 Plus", "I (2018–2021)", "I Рестайлинг (2021–н.в.)"),
            new("CS55 Plus", "II (2021–н.в.)"),
            new("UNI-V", "I (2021–н.в.)"),
            new("UNI-K", "I (2020–н.в.)")
        },
        ["Nissan"] = new()
        {
            new("Qashqai", "I (J10, 2006–2013)", "II (J11, 2013–2021)", "III (J12, 2021–н.в.)"),
            new("X-Trail", "II (T31, 2007–2014)", "III (T32, 2013–2022)", "IV (T33, 2021–н.в.)"),
            new("Almera", "Classic (2006–2013)", "G15 (2012–2018)"),
            new("Teana", "II (J32, 2008–2014)", "III (J33, 2013–2020)")
        },
        ["Ford"] = new()
        {
            new("Focus", "II (2004–2011)", "III (2011–2019)"),
            new("Mondeo", "IV (2007–2015)", "V (2014–2022)"),
            new("Kuga", "I (2008–2012)", "II (2012–2019)")
        },
        ["Mazda"] = new()
        {
            new("3", "I (BK, 2003–2009)", "II (BL, 2008–2013)", "III (BM, 2013–2019)", "IV (BP, 2019–н.в.)"),
            new("6", "I (GG, 2002–2008)", "II (GH, 2007–2013)", "III (GJ/GL, 2012–н.в.)"),
            new("CX-5", "I (KE, 2011–2017)", "II (KF, 2016–н.в.)")
        }
    };

    // Список всех марок
    public static IReadOnlyList<string> PopularBrands => Catalog.Keys.OrderBy(x => x).ToList();

    // Получить список моделей для выбранной марки
    public static IReadOnlyList<string> GetModelsForBrand(string? brand)
    {
        if (string.IsNullOrWhiteSpace(brand)) return Array.Empty<string>();

        // Ищем точное или частичное совпадение марки
        var match = Catalog.Keys.FirstOrDefault(k => k.Equals(brand.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                                    brand.Trim().StartsWith(k, StringComparison.OrdinalIgnoreCase));
        if (match != null && Catalog.TryGetValue(match, out var models))
        {
            return models.Select(m => m.Name).ToList();
        }

        return Array.Empty<string>();
    }

    // Получить список поколений для выбранной марки и модели
    public static IReadOnlyList<string> GetGenerationsForModel(string? brand, string? model)
    {
        if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model)) 
            return Array.Empty<string>();

        var matchBrand = Catalog.Keys.FirstOrDefault(k => k.Equals(brand.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                                         brand.Trim().StartsWith(k, StringComparison.OrdinalIgnoreCase));
        if (matchBrand != null && Catalog.TryGetValue(matchBrand, out var models))
        {
            var matchModel = models.FirstOrDefault(m => m.Name.Equals(model.Trim(), StringComparison.OrdinalIgnoreCase));
            if (matchModel != null)
            {
                return matchModel.Generations;
            }
        }

        return Array.Empty<string>();
    }
}
