namespace ecommerce.Data
{
    public class StaticEnum { }

    public enum Countries
    {
        Afghanistan,
        Albania,
        Algeria,
        Andorra,
        Angola,
        AntiguaDeps,
        Argentina,
        Armenia,
        Australia,
        Austria,
        Azerbaijan,
        Bahamas,
        Bahrain,
        Bangladesh,
        Barbados,
        Belarus,
        Belgium,
        Belize,
        Benin,
        Bhutan,
        Bolivia,
        BosniaHerzegovina,
        Botswana,
        Brazil,
        Brunei,
        Bulgaria,
        Burkina,
        Burundi,
        Cambodia,
        Cameroon,
        Canada,
        CapeVerde,
        CentralAfricanRep,
        Chad,
        Chile,
        China,
        Colombia,
        Comoros,
        Congo,
        CostaRica,
        Croatia,
        Cuba,
        Cyprus,
        Czech,
        Denmark,
        Djibouti,
        Dominica,
        DominicanRepublic,
        EastTimor,
        Ecuador,
        Egypt,
        ElSalvador,
        EquatorialGuinea,
        Eritrea,
        Estonia,
        Ethiopia,
        Fiji,
        Finland,
        France,
        Gabon,
        Gambia,
        Georgia,
        Germany,
        Ghana,
        Greece,
        Grenada,
        Guatemala,
        Guinea,
        GuineaBissau,
        Guyana,
        Haiti,
        Honduras,
        Hungary,
        Iceland,
        India,
        Indonesia,
        Iran,
        Iraq,
        Ireland,
        Israel,
        Italy,
        IvoryCoast,
        Jamaica,
        Japan,
        Jordan,
        Kazakhstan,
        Kenya,
        Kiribati,
        KoreaNorth,
        KoreaSouth,
        Kosovo,
        Kuwait,
        Kyrgyzstan,
        Laos,
        Latvia,
        Lebanon,
        Lesotho,
        Liberia,
        Libya,
        Liechtenstein,
        Lithuania,
        Luxembourg,
        Macedonia,
        Madagascar,
        Malawi,
        Malaysia,
        Maldives,
        Mali,
        Malta,
        MarshallIslands,
        Mauritania,
        Mauritius,
        Mexico,
        Micronesia,
        Moldova,
        Monaco,
        Mongolia,
        Montenegro,
        Morocco,
        Mozambique,
        Myanmar,
        Namibia,
        Nauru,
        Nepal,
        Netherlands,
        NewZealand,
        Nicaragua,
        Niger,
        Nigeria,
        Norway,
        Oman,
        Pakistan,
        Palau,
        Panama,
        PapuaNewGuinea,
        Paraguay,
        Peru,
        Philippines,
        Poland,
        Portugal,
        Qatar,
        Romania,
        Russian,
        Rwanda,
        StKittNevis,
        StLucia,
        SaintVincentTheGrenadines,
        Samoa,
        SanMarino,
        SaoTomePrincipe,
        SaudiArabia,
        Senegal,
        Serbia,
        Seychelles,
        SierraLeone,
        Singapore,
        Slovakia,
        Slovenia,
        SolomonIslands,
        Somalia,
        SouthAfrica,
        SouthSudan,
        Spain,
        SriLanka,
        Sudan,
        Suriname,
        Swaziland,
        Sweden,
        Switzerland,
        Syria,
        Taiwan,
        Tajikistan,
        Tanzania,
        Thailand,
        Togo,
        Tonga,
        TrinidadTobago,
        Tunisia,
        Turkey,
        Turkmenistan,
        Tuvalu,
        Uganda,
        Ukraine,
        UAE,
        UK,
        USA,
        Uruguay,
        Uzbekistan,
        Vanuatu,
        VaticanCity,
        Venezuela,
        Vietnam,
        Yemen,
        Zambia,
        Zimbabwe
    }

    public enum MediaType
    {
        Video = 1,
        Image = 2,
        File = 3,
        Folder = 4
    }

    public enum Genders
    {
        Male = 1,
        Female = 2,
        NA = 3
    }

    public enum Status
    {
        Open = 1,
        Pending = 2,
        Closed = 3,
        Returned = 4,
        Deleted = 5
    }

    public enum TrackingStatus
    {
        Ordered = 1,
        PickedUp = 2,
        Dispatched = 3,
        InTransit = 4,
        Delivered = 5,
        Deleted = 6
    }

    //If you need to add more methods you have to change the AZ enum too..
    public enum PaymentMethod
    {
        Card = 1,
        Cash = 2
    }

    public enum PackageType
    {
        Kit = 1,
        Set = 2,
        Bottle = 3,
        Box = 4,
        Pack = 5,
        Carton = 6,
        Unit = 7,
        Each = 8,
        Gram = 9
    }

    public enum AlertType
    {
        Danger = 1,
        Warning = 2,
        Success = 3
    }

    public enum MetaTagType
    {
        Site = 1,
        Product = 2
    }

    public enum Navs
    {
        Main = 1,
        Information = 2,
        Extras = 3
    }

    public enum DiscountType
    {
        Percentage = 1,
        FixedAmount = 2,
        FreeShipping = 3
    }
}
