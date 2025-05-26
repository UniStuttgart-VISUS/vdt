// <copyright file="InputProfiles.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;


namespace Visus.DeploymentToolkit.SystemInformation {

    /// <summary>
    /// Enumerates the input profiles from 
    /// https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/default-input-locales-for-windows-language-packs?view=windows-11
    /// </summary>
    public static class InputProfiles {

        public static string? ForCulture(CultureInfo? culture) {
            if (culture is null) {
                return null;
            }

            var flags = BindingFlags.Public | BindingFlags.Static;
            var fields = from f in typeof(InputProfiles).GetFields(flags)
                         where f.FieldType == typeof(string)
                         let a = f.GetCustomAttribute<CultureAttribute>()
                         where a is not null && a.Culture.Equals(culture)
                         select f;
            return fields.FirstOrDefault()?.GetValue(null) as string;
        }

        [Culture("af-ZA")]
        public const string Afrikaans = "0436:00000409";

        public const string AfrikaansNamibia = "0409:00000409";

        [Culture("sq-AL")]
        public const string Albanian = "041C:0000041C";

        public const string AlbanianKosovo = "0409:00000409";

        [Culture("gsw-FR")]
        public const string AlsatianFrance = "0484:0000040C";

        [Culture("am-ET")]
        public const string Amharic = "045E:"
            + "{7C472071-36A7-4709-88CC-859513E583A9}"
            + "{9A4E8FC7-76BF-4A63-980D-FADDADF7E987}";

        [Culture("ar-SA")]
        public const string Arabic = "0401:00000401";

        [Culture("ar-DZ")]
        public const string ArabicAlgeria = "1401:00020401";

        [Culture("ar-BH")]
        public const string ArabicBahrain = "3C01:00000401";

        [Culture("ar-EG")]
        public const string ArabicEgypt = "0C01:00000401";

        [Culture("ar-IQ")]
        public const string ArabicIraq = "0801:00000401";

        [Culture("ar-JO")]
        public const string ArabicJordan = "2C01:00000401";

        [Culture("ar-KW")]
        public const string ArabicKuwait = "3401:00000401";

        [Culture("ar-LB")]
        public const string ArabicLebanon = "3001:00000401";

        [Culture("ar-LY")]
        public const string ArabicLibya = "1001:00000401";

        [Culture("ar-MA")]
        public const string ArabicMorocco = "1801:00020401";

        [Culture("ar-OM")]
        public const string ArabicOman = "2001:00000401";

        [Culture("ar-QA")]
        public const string ArabicQatar = "4001:00000401";

        [Culture("ar-SY")]
        public const string ArabicSyria = "2801:00000401";

        [Culture("ar-TN")]
        public const string ArabicTunisia = "1C01:00020401";

        [Culture("ar-AE")]
        public const string ArabicUnitedArabEmirates = "3801:00000401";

        public const string ArabicWorld = "0409:00000409";

        [Culture("ar-YE")]
        public const string ArabicYemen = "2401:00000401";

        [Culture("hy-AM")]
        public const string Armenian = "042B:0002042B";

        [Culture("as-IN")]
        public const string Assamese = "044D:0000044D";

        [Culture("es-ES_tradnl")]
        public const string Asturian = "040A:0000040A";

        [Culture("az-Latn-AZ")]
        public const string Azerbaijani = "042C:0000042C";

        [Culture("az-Cyrl-AZ")]
        public const string AzerbaijaniCyrillic = "082C:0000082C";

        [Culture("bn-BD")]
        public const string Bangla = "0845:00000445";

        [Culture("ba-RU")]
        public const string Bashkir = "046D:0000046D";

        [Culture("eu-ES")]
        public const string Basque = "042D:0000040A";

        [Culture("be-BY")]
        public const string Belarusian = "0423:00000423";

        [Culture("bn-IN")]
        public const string BengaliIndia = "0445:00020445";

        [Culture("hi-IN")]
        public const string Bodo = "0439:00000439";

        [Culture("bs-Latn-BA")]
        public const string Bosnian = "141A:0000041A";

        [Culture("bs-Cyrl-BA")]
        public const string BosnianCyrillic = "201A:0000201A";

        [Culture("br-FR")]
        public const string Breton = "047E:0000040C";

        [Culture("bg-BG")]
        public const string Bulgarian = "0402:00030402";

        [Culture("my-MM")]
        public const string Burmese = "0455:00130C00";

        [Culture("ca-ES")]
        public const string Catalan = "0403:0000040A";

        public const string CatalanAndorra = "040C:0000040C";

        public const string CatalanFrance = "040C:0000040C";

        public const string CatalanItaly = "0410:00000410";

        [Culture("tzm-Latn-DZ")]
        public const string CentralAtlasTamazight = "085F:0000085F";

        public const string CentralAtlasTamazightArabic = "1801:00020401";

        [Culture("tzm-Tfng-MA")]
        public const string CentralAtlasTamazightTifinagh = "105F:0000105F";

        [Culture("ku-Arab-IQ")]
        public const string CentralKurdish = "0492:00000492";

        public const string Chechen = "0419:00000419";

        [Culture("chr-Cher-US")]
        public const string Cherokee = "045C:0000045C";

        [Culture("zh-CN")]
        public const string Chinese = "0804:"
            + "{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}"
            + "{FA550B04-5AD7-411F-A5AC-CA038EC515D7}";

        public const string ChineseSimplifiedHongKongSAR = "0409:00000409";

        public const string ChineseSimplifiedMacaoSAR = "0409:00000409";

        [Culture("zh-TW")]
        public const string ChineseTraditional = "0404:"
            + "{531FDEBF-9B4C-4A43-A2AA-960E8FCDC732}"
            + "{6024B45F-5C54-11D4-B921-0080C882687E}";

        [Culture("zh-TW")]
        public const string ChineseTraditionalTaiwan = "0404:"
            + "{B115690A-EA02-48D5-A231-E3578D2FDF80}"
            + "{B2F9C502-1742-11D4-9790-0080C882687E}";

        public const string ChurchSlavic = "0419:00000419";

        public const string Colognian = "0407:00000407";

        public const string Cornish = "0809:00000809";

        [Culture("co-FR")]
        public const string Corsican = "0483:0000040C";

        [Culture("hr-HR")]
        public const string Croatian = "041A:0000041A";

        [Culture("hr-BA")]
        public const string CroatianBosniaHerzegovina = "101A:0000041A";

        [Culture("cs-CZ")]
        public const string Czech = "0405:00000405";

        [Culture("da-DK")]
        public const string Danish = "0406:00000406";

        public const string DanishGreenland = "0406:00000406";

        [Culture("dv-MV")]
        public const string Divehi = "0465:00000465";

        [Culture("nl-NL")]
        public const string Dutch = "0413:00020409";

        public const string DutchAruba = "0409:00020409";

        public const string DutchBelgium = "0813:00000813";

        public const string DutchBonaireSintEustatiusandSaba = "0409:00020409";

        public const string DutchCuraτao = "0409:00020409";

        public const string DutchSintMaarten = "0409:00020409";

        public const string DutchSuriname = "0409:00020409";

        [Culture("dz-BT")]
        public const string Dzongkha = "0C51:00000C51";

        [Culture("en-US")]
        public const string English = "0409:00000409";

        [Culture("en-AU")]
        public const string EnglishAustralia = "0C09:00000409";

        [Culture("en-GB")]
        public const string EnglishAustria = "0809:00000407";

        [Culture("fr-BE")]
        public const string EnglishBelgium = "080C:0000080C";

        [Culture("en-BZ")]
        public const string EnglishBelize = "2809:00000409";

        public const string EnglishBritishVirginIslands = "0809:00000809";

        public const string EnglishBurundi = "0809:0000040C";

        [Culture("en-CA")]
        public const string EnglishCanada = "1009:00000409";

        [Culture("en-029")]
        public const string EnglishCaribbean = "2409:00000409";

        public const string EnglishDenmark = "0409:00000406";

        public const string EnglishFalklandIslands = "0809:00000809";

        public const string EnglishFinland = "0809:0000040B";

        public const string EnglishGermany = "0809:00000407";

        public const string EnglishGibraltar = "0809:00000809";

        public const string EnglishGuernsey = "0809:00000809";

        [Culture("en-HK")]
        public const string EnglishHongKongSAR = "3C09:00000409";

        [Culture("en-IN")]
        public const string EnglishIndia = "4009:00004009";

        [Culture("en-IE")]
        public const string EnglishIreland = "1809:00001809";

        public const string EnglishIsleofMan = "0809:00000809";

        public const string EnglishIsrael = "0409:00000409";

        [Culture("en-JM")]
        public const string EnglishJamaica = "2009:00000409";

        public const string EnglishJersey = "0809:00000809";

        [Culture("en-MY")]
        public const string EnglishMalaysia = "4409:00000409";

        public const string EnglishMalta = "0809:00000809";

        public const string EnglishNetherlands = "0809:00020409";

        [Culture("en-NZ")]
        public const string EnglishNewZealand = "1409:00001409";

        [Culture("en-PH")]
        public const string EnglishPhilippines = "3409:00000409";

        [Culture("en-SG")]
        public const string EnglishSingapore = "4809:00000409";

        public const string EnglishSlovenia = "0809:00000424";

        [Culture("en-ZA")]
        public const string EnglishSouthAfrica = "1C09:00000409";

        public const string EnglishSweden = "0809:0000041D";

        public const string EnglishSwitzerland = "0809:00000407";

        [Culture("en-TT")]
        public const string EnglishTrinidadTobago = "2C09:00000409";

        [Culture("en-GB")]
        public const string EnglishUnitedKingdom = "0809:00000809";

        [Culture("en-ZW")]
        public const string EnglishZimbabwe = "3009:00000409";

        [Culture("et-EE")]
        public const string Estonian = "0425:00000425";

        [Culture("fo-FO")]
        public const string Faroese = "0438:00000406";

        [Culture("fil-PH")]
        public const string Filipino = "0464:00000409";

        [Culture("fi-FI")]
        public const string Finnish = "040B:0000040B";

        [Culture("fr-FR")]
        public const string French = "040C:0000040C";

        [Culture("fr-BE")]
        public const string FrenchBelgium = "080C:0000080C";

        [Culture("fr-CM")]
        public const string FrenchCameroon = "2C0C:0000040C";

        [Culture("fr-CA")]
        public const string FrenchCanada = "0C0C:00001009";

        [Culture("fr-CD")]
        public const string FrenchCongoDRC = "240C:0000040C";

        [Culture("fr-CI")]
        public const string FrenchCoteDIvoire = "300C:0000040C";

        [Culture("fr-HT")]
        public const string FrenchHaiti = "3C0C:0000040C";

        [Culture("fr-LU")]
        public const string FrenchLuxembourg = "140C:0000100C";

        [Culture("fr-ML")]
        public const string FrenchMali = "340C:0000040C";

        [Culture("fr-MC")]
        public const string FrenchMonaco = "180C:0000040C";

        [Culture("fr-MA")]
        public const string FrenchMorocco = "380C:0000040C";

        [Culture("fr-RE")]
        public const string FrenchReunion = "200C:0000040C";

        [Culture("fr-SN")]
        public const string FrenchSenegal = "280C:0000040C";

        [Culture("fr-CH")]
        public const string FrenchSwitzerland = "100C:0000100C";

        public const string Friulian = "0410:00000410";

        [Culture("ff-Latn-SN")]
        public const string Fulah = "0867:00000488";

        public const string FulahAdlam = "0409:00000409";

        public const string FulahLatinBurkinaFaso = "0409:00000409";

        [Culture("wo-SN")]
        public const string FulahLatinCameroon = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinGambia = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinGhana = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinGuinea = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinGuineaBissau = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinLiberia = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinMauritania = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinNiger = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinNigeria = "0488:00000488";

        [Culture("wo-SN")]
        public const string FulahLatinSierraLeone = "0488:00000488";

        [Culture("gl-ES")]
        public const string Galician = "0456:0000040A";

        [Culture("ka-GE")]
        public const string Georgian = "0437:00010437";

        [Culture("de-DE")]
        public const string German = "0407:00000407";

        [Culture("de-AT")]
        public const string GermanAustria = "0C07:00000407";

        public const string GermanBelgium = "080C:0000080C";

        public const string GermanItaly = "0409:00000409";
        
        [Culture("de-LI")]
        public const string GermanLiechtenstein = "1407:00000807";

        [Culture("de-LU")]
        public const string GermanLuxembourg = "1007:00000407";

        [Culture("de-CH")]
        public const string GermanSwitzerland = "0807:00000807";

        [Culture("el-GR")]
        public const string Greek = "0408:00000408";

        [Culture("gn-PY")]
        public const string Guarani = "0474:00000474";

        [Culture("gu-IN")]
        public const string Gujarati = "0447:00000447";

        [Culture("ha-Latn-NG")]
        public const string Hausa = "0468:00000468";

        [Culture("haw-US")]
        public const string Hawaiian = "0475:00000475";

        [Culture("he-IL")]
        public const string Hebrew = "040D:0002040D";

        [Culture("hi-IN")]
        public const string Hindi = "0439:00010439";

        [Culture("hu-HU")]
        public const string Hungarian = "040E:0000040E";

        [Culture("is-IS")]
        public const string Icelandic = "040F:0000040F";

        [Culture("ig-NG")]
        public const string Igbo = "0470:00000470";

        [Culture("id-ID")]
        public const string Indonesian = "0421:00000409";

        public const string Interlingua = "040C:0000040C";

        [Culture("iu-Latn-CA")]
        public const string Inuktitut = "085D:0000085D";
        
        [Culture("iu-Cans-CA")]
        public const string InuktitutSyllabics = "045D:0001045D";
        
        [Culture("ga-IE")]
        public const string Irish = "083C:00001809";
        
        public const string IrishUnitedKingdom = "0409:00000409";
        
        [Culture("xh-ZA")]
        public const string IsiXhosa = "0434:00000409";

        [Culture("zu-ZA")]
        public const string IsiZulu = "0435:00000409";

        [Culture("it-IT")]
        public const string Italian = "0410:00000410";

        [Culture("it-CH")]
        public const string ItalianSwitzerland = "0810:0000100C";

        public const string ItalianVaticanCity = "0409:00000409";

        [Culture("ja-JP")]
        public const string Japanese = "0411:"
            + "{03B5835F-F03C-411B-9CE2-AA23E1171E36}"
            + "{A76C93D9-5523-4E90-AAFA-4DB112F9AC76}";

        public const string Javanese = "0C00:00000409";

        [Culture("jv-Java")]
        public const string JavaneseJavanese = "0C00:00110C00";

        [Culture("kl-GL")]
        public const string Kalaallisut = "046F:00000406";

        [Culture("kn-IN")]
        public const string Kannada = "044B:0000044B";

        [Culture("ur-PK")]
        public const string Kashmiri = "0420:00000420";

        [Culture("hi-IN")]
        public const string KashmiriDevanagari = "0439:00010439";

        [Culture("kk-KZ")]
        public const string Kazakh = "043F:0000043F";

        [Culture("km-KH")]
        public const string Khmer = "0453:00000453";

        [Culture("quc-Latn-GT")]
        public const string Kiche = "0486:0000080A";

        [Culture("rw-RW")]
        public const string Kinyarwanda = "0487:00000409";

        [Culture("sw-KE")]
        public const string Kiswahili = "0441:00000409";

        public const string KiswahiliCongoDRC = "0409:00000409";

        public const string KiswahiliTanzania = "0409:00000409";

        public const string KiswahiliUganda = "0409:00000409";

        [Culture("kok-IN")]
        public const string Konkani = "0457:00000439";

        [Culture("ko-KR")]
        public const string Korean = "0412:"
            + "{A028AE76-01B1-46C2-99C4-ACD9858AE02F}"
            + "{B5FE1F02-D5F2-4445-9C03-C568F23C99A1}";

        public const string KoreanNorthKorea = "0409:00000409";

        [Culture("ky-KG")]
        public const string Kyrgyz = "0440:00000440";

        [Culture("lo-LA")]
        public const string Lao = "0454:00000454";

        [Culture("lv-LV")]
        public const string Latvian = "0426:00020426";

        [Culture("lt-LT")]
        public const string Lithuanian = "0427:00010427";

        [Culture("dsb-DE")]
        public const string LowerSorbian = "082E:0002042E";

        [Culture("lb-LU")]
        public const string Luxembourgish = "046E:0000046E";

        [Culture("mk-MK")]
        public const string Macedonian = "042F:0001042F";

        public const string Malagasy = "0C00:0000040C";

        [Culture("ms-MY")]
        public const string Malay = "043E:00000409";

        [Culture("ml-IN")]
        public const string Malayalam = "044C:0000044C";

        [Culture("ms-BN")]
        public const string MalayBrunei = "083E:00000409";

        public const string MalayIndonesia = "0409:00000409";

        public const string MalaySingapore = "0409:00000409";

        [Culture("mt-MT")]
        public const string Maltese = "043A:0000043A";

        public const string Manipuri = "4009:00004009";

        public const string Manx = "0809:00000809";

        [Culture("mi-NZ")]
        public const string Maori = "0481:00000481";

        [Culture("arn-CL")]
        public const string Mapuche = "047A:0000080A";

        [Culture("mr-IN")]
        public const string Marathi = "044E:0000044E";

        [Culture("fa-IR")]
        public const string Mazanderani = "0429:00000429";

        [Culture("moh-CA")]
        public const string Mohawk = "047C:00000409";

        [Culture("mn-MN")]
        public const string Mongolian = "0450:00000450";

        [Culture("mn-Mong-CN")]
        public const string MongolianTraditionalMongolian = "0850:00010850";

        [Culture("mn-Mong-MN")]
        public const string MongolianTraditionalMongolianMongolia
            = "0C50:00010850";

        [Culture("ne-NP")]
        public const string Nepali = "0461:00000461";

        [Culture("ne-IN")]
        public const string NepaliIndia = "0861:00000461";

        public const string Nko = "0C00:00090C00";

        [Culture("ar-IQ")]
        public const string NorthernLuri = "0801:00000401";

        [Culture("se-NO")]
        public const string NorthernSami = "043B:0000043B";

        [Culture("nb-NO")]
        public const string Norwegian = "0414:00000414";

        [Culture("nn-NO")]
        public const string NorwegianNynorsk = "0814:00000414";

        [Culture("oc-FR")]
        public const string Occitan = "0482:0000040C";

        [Culture("or-IN")]
        public const string Odia = "0448:00000448";

        [Culture("om-ET")]
        public const string Oromo = "0472:00000409";

        public const string OromoKenya = "0409:00000409";

        public const string Ossetic = "0419:00000419";

        [Culture("pap-029")]
        public const string Papiamento = "0479:00000409";

        [Culture("ps-AF")]
        public const string Pashto = "0463:00000463";

        public const string PashtoPakistan = "0409:00000409";

        [Culture("fa-IR")]
        public const string Persian = "0429:00000429";

        [Culture("fa-AF")]
        public const string PersianAfghanistan = "048C:00050429";

        [Culture("pl-PL")]
        public const string Polish = "0415:00000415";

        [Culture("pt-BR")]
        public const string Portuguese = "0416:00000416";

        public const string PortugueseAngola = "0816:00000816";

        public const string PortugueseCaboVerde = "0816:00000816";

        public const string PortugueseEquatorialGuinea = "0409:00000409";

        public const string PortugueseGuineaBissau = "0816:00000816";

        public const string PortugueseLuxembourg = "0409:00000409";

        public const string PortugueseMacaoSAR = "0816:00000816";

        public const string PortugueseMozambique = "0816:00000816";

        public const string PortuguesePortugal = "0816:00000816";

        public const string PortugueseSwitzerland = "0409:00000409";

        public const string PortugueseSaoTomePrincipe = "0816:00000816";

        public const string PortugueseTimorLeste = "0816:00000816";

        public const string Prussian = "0407:00000407";

        //public const string Punjabi = "0446:00000446";

        [Culture("pa-Arab-PK")]
        public const string Punjabi = "0846:00000420";

        [Culture("quz-BO")]
        public const string Quechua = "046B:0000080A";

        [Culture("quz-EC")]
        public const string QuechuaEcuador = "086B:0000080A";

        [Culture("quz-PE")]
        public const string QuechuaPeru = "0C6B:0000080A";

        [Culture("ro-RO")]
        public const string Romanian = "0418:00010418";

        [Culture("ro-MD")]
        public const string RomanianMoldova = "0818:00010418";

        [Culture("rm-CH")]
        public const string Romansh = "0417:00000807";

        [Culture("ru-RU")]
        public const string Russian = "0419:00000419";

        public const string RussianMoldova = "0409:00000409";

        [Culture("sah-RU")]
        public const string Sakha = "0485:00000485";

        [Culture("smn-FI")]
        public const string SamiInari = "243B:0001083B";

        [Culture("smj-SE")]
        public const string SamiLule = "143B:0000083B";

        [Culture("smj-NO")]
        public const string SamiLuleNorway = "103B:0000043B";

        [Culture("se-FI")]
        public const string SamiNorthernFinland = "0C3B:0001083B";

        [Culture("se-SE")]
        public const string SamiNorthernSweden = "083B:0000083B";

        [Culture("sms-FI")]
        public const string SamiSkolt = "203B:0001083B";

        [Culture("sma-SE")]
        public const string SamiSouthern = "1C3B:0000083B";

        [Culture("sma-NO")]
        public const string SamiSouthernNorway = "183B:0000043B";

        [Culture("sa-IN")]
        public const string Sanskrit = "044F:00000439";

        [Culture("gd-GB")]
        public const string ScottishGaelic = "0491:00011809";

        [Culture("sr-Latn-RS")]
        public const string Serbian = "241A:0000081A";

        [Culture("sr-Cyrl-RS")]
        public const string SerbianCyrillic = "281A:00000C1A";

        [Culture("sr-Cyrl-BA")]
        public const string SerbianCyrillicBosniaandHerzegovina
            = "1C1A:00000C1A";

        public const string SerbianCyrillicKosovo = "0409:00000409";

        [Culture("sr-Cyrl-ME")]
        public const string SerbianCyrillicMontenegro = "301A:00000C1A";

        [Culture("sr-Latn-BA")]
        public const string SerbianLatinBosniaHerzegovina = "181A:0000081A";

        public const string SerbianLatinKosovo = "0409:00000409";

        [Culture("sr-Latn-ME")]
        public const string SerbianLatinMontenegro = "2C1A:0000081A";

        [Culture("st-ZA")]
        public const string Sesotho = "0430:00000409";

        public const string SesothoLesotho = "0409:00000409";

        [Culture("nso-ZA")]
        public const string SesothosaLeboa = "046C:0000046C";

        [Culture("tn-ZA")]
        public const string Setswana = "0432:00000432";

        [Culture("tn-BW")]
        public const string SetswanaBotswana = "0832:00000432";

        public const string Shona = "0C00:00000409";

        [Culture("sd-Arab-PK")]
        public const string Sindhi = "0859:00000420";

        [Culture("hi-IN")]
        public const string SindhiDevanagari = "0439:00010439";

        [Culture("si-LK")]
        public const string Sinhala = "045B:0000045B";

        [Culture("sk-SK")]
        public const string Slovak = "041B:0000041B";

        [Culture("sl-SI")]
        public const string Slovenian = "0424:00000424";

        [Culture("so-SO")]
        public const string Somali = "0477:00000409";

        public const string SomaliDjibouti = "0409:00000409";

        public const string SomaliEthiopia = "0409:00000409";

        public const string SomaliKenya = "0409:00000409";

        [Culture("es-ES")]
        public const string Spanish = "0C0A:0000040A";

        [Culture("es-AR")]
        public const string SpanishArgentina = "2C0A:0000080A";

        public const string SpanishBelize = "0409:00000409";

        [Culture("es-BO")]
        public const string SpanishBolivia = "400A:0000080A";

        public const string SpanishBrazil = "0409:00000409";

        [Culture("es-CL")]
        public const string SpanishChile = "340A:0000080A";

        [Culture("es-CO")]
        public const string SpanishColombia = "240A:0000080A";

        [Culture("es-CR")]
        public const string SpanishCostaRica = "140A:0000080A";

        [Culture("es-MX")]
        public const string SpanishCuba = "080A:0000080A";

        [Culture("es-DO")]
        public const string SpanishDominicanRepublic = "1C0A:0000080A";

        [Culture("es-EC")]
        public const string SpanishEcuador = "300A:0000080A";

        [Culture("es-SV")]
        public const string SpanishElSalvador = "440A:0000080A";

        [Culture("es-MX")]
        public const string SpanishEquatorialGuinea = "080A:0000080A";

        [Culture("es-GT")]
        public const string SpanishGuatemala = "100A:0000080A";

        [Culture("es-HN")]
        public const string SpanishHonduras = "480A:0000080A";

        [Culture("es-419")]
        public const string SpanishLatinAmerica = "580A:0000080A";

        [Culture("es-MX")]
        public const string SpanishMexico = "080A:0000080A";

        [Culture("es-NI")]
        public const string SpanishNicaragua = "4C0A:0000080A";

        [Culture("es-PA")]
        public const string SpanishPanama = "180A:0000080A";

        [Culture("es-PY")]
        public const string SpanishParaguay = "3C0A:0000080A";

        [Culture("es-PE")]
        public const string SpanishPeru = "280A:0000080A";

        public const string SpanishPhilippines = "080A:0000080A";

        [Culture("es-PR")]
        public const string SpanishPuertoRico = "500A:0000080A";

        public const string SpanishUnitedStates = "540A:0000080A";

        [Culture("es-UY")]
        public const string SpanishUruguay = "380A:0000080A";

        [Culture("es-VE")]
        public const string SpanishVenezuela = "200A:0000080A";

        public const string StandardMoroccanTamazight = "0C00:0000105F";

        [Culture("sv-SE")]
        public const string Swedish = "041D:0000041D";

        [Culture("sv-FI")]
        public const string SwedishFinland = "081D:0000041D";

        [Culture("de-CH")]
        public const string SwissGerman = "0807:00000807";

        [Culture("syr-SY")]
        public const string Syriac = "045A:0000045A";

        [Culture("tzm-Tfng-MA")]
        public const string Tachelhit = "105F:0000105F";

        [Culture("tzm-Latn-DZ")]
        public const string TachelhitLatin = "085F:0000085F";

        [Culture("tg-Cyrl-TJ")]
        public const string Tajik = "0428:00000428";

        [Culture("ta-IN")]
        public const string Tamil = "0449:00020449";

        [Culture("ta-IN")]
        public const string TamilMalaysia = "0449:00020449";

        [Culture("ta-IN")]
        public const string TamilSingapore = "0449:00020449";

        [Culture("ta-LK")]
        public const string TamilSriLanka = "0849:00020449";

        [Culture("tt-RU")]
        public const string Tatar = "0444:00010444";

        [Culture("te-IN")]
        public const string Telugu = "044A:0000044A";

        [Culture("th-TH")]
        public const string Thai = "041E:0000041E";

        [Culture("bo-CN")]
        public const string Tibetan = "0451:00010451";

        [Culture("bo-CN")]
        public const string TibetanIndia = "0451:00000451";

        [Culture("ti-ET")]
        public const string Tigrinya = "0473:"
            + "{E429B25A-E5D3-4D1F-9BE3-0C608477E3A1}"
            + "{3CAB88B7-CC3E-46A6-9765-B772AD7761FF}";

        [Culture("tr-TR")]
        public const string Turkish = "041F:0000041F";

        [Culture("tk-TM")]
        public const string Turkmen = "0442:00000442";

        [Culture("uk-UA")]
        public const string Ukrainian = "0422:00020422";

        [Culture("hsb-DE")]
        public const string UpperSorbian = "042E:0002042E";

        [Culture("ur-PK")]
        public const string Urdu = "0420:00000420";

        [Culture("ur-IN")]
        public const string UrduIndia = "0820:00000420";

        [Culture("ug-CN")]
        public const string Uyghur = "0480:00010480";

        [Culture("uz-Latn-UZ")]
        public const string Uzbek = "0443:00000409";

        [Culture("ps-AF")]
        public const string UzbekArabic = "0463:00000463";

        [Culture("uz-Cyrl-UZ")]
        public const string UzbekCyrillic = "0843:00000843";

        [Culture("ca-ES-valencia")]
        public const string ValencianSpain = "0803:0000040A";

        [Culture("vi-VN")]
        public const string Vietnamese = "042A:"
            + "{C2CB2CF0-AF47-413E-9780-8BC3A3C16068}"
            + "{5FB02EC5-0A77-4684-B4FA-DEF8A2195628}";

        [Culture("de-CH")]
        public const string Walser = "0807:00000807";

        [Culture("cy-GB")]
        public const string Welsh = "0452:00000452";

        [Culture("fy-NL")]
        public const string WesternFrisian = "0462:00020409";

        [Culture("wo-SN")]
        public const string Wolof = "0488:00000488";

        [Culture("ts-ZA")]
        public const string Xitsonga = "0431:00000409";

        [Culture("ii-CN")]
        public const string Yi = "0478:"
            + "{E429B25A-E5D3-4D1F-9BE3-0C608477E3A1}"
            + "{409C8376-007B-4357-AE8E-26316EE3FB0D}";

        [Culture("yo-NG")]
        public const string Yoruba = "046A:0000046A";
    }
}
