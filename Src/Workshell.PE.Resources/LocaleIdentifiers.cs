#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources
{

    public sealed  class LocaleIdentifiers
    {

        public const ushort SUBLANG_CUSTOM_DEFAULT = 0x0C00;
        public const ushort SUBLANG_UI_CUSTOM_DEFAULT = 0x1400;
        public const ushort SUBLANG_NEUTRAL_INVARIANT = 0x007F;
        public const ushort SUBLANG_NEUTRAL = 0x0000;
        public const ushort SUBLANG_SYS_DEFAULT = 0x0800;
        public const ushort SUBLANG_CUSTOM_UNSPECIFIED = 0x1000;
        public const ushort SUBLANG_DEFAULT = 0x0400;

        public const ushort SUBLANG_AFRIKAANS_SOUTH_AFRICA = 0x0436;
        public const ushort SUBLANG_ALBANIAN_ALBANIA = 0x041C;
        public const ushort SUBLANG_ALSATIAN_FRANCE = 0x0484;
        public const ushort SUBLANG_AMHARIC_ETHIOPIA = 0x045E;
        public const ushort SUBLANG_ARABIC_ALGERIA = 0x1401;
        public const ushort SUBLANG_ARABIC_BAHRAIN = 0x3C01;
        public const ushort SUBLANG_ARABIC_EGYPT = 0x0C01;
        public const ushort SUBLANG_ARABIC_IRAQ = 0x0801;
        public const ushort SUBLANG_ARABIC_JORDAN = 0x2C01;
        public const ushort SUBLANG_ARABIC_KUWAIT = 0x3401;
        public const ushort SUBLANG_ARABIC_LEBANON = 0x3001;
        public const ushort SUBLANG_ARABIC_LIBYA = 0x1001;
        public const ushort SUBLANG_ARABIC_MOROCCO = 0x1801;
        public const ushort SUBLANG_ARABIC_OMAN = 0x2001;
        public const ushort SUBLANG_ARABIC_QATAR = 0x4001;
        public const ushort SUBLANG_ARABIC_SAUDI_ARABIA = 0x0401;
        public const ushort SUBLANG_ARABIC_SYRIA = 0x2801;
        public const ushort SUBLANG_ARABIC_TUNISIA = 0x1C01;
        public const ushort SUBLANG_ARABIC_UAE = 0x3801;
        public const ushort SUBLANG_ARABIC_YEMEN = 0x2401;
        public const ushort SUBLANG_ARMENIAN_ARMENIA = 0x042B;
        public const ushort SUBLANG_ASSAMESE_INDIA = 0x044D;
        public const ushort SUBLANG_AZERI_CYRILLIC = 0x082C;
        public const ushort SUBLANG_AZERI_LATIN = 0x042C;
        public const ushort SUBLANG_BANGLA_BANGLADESH = 0x0445;
        public const ushort SUBLANG_BASHKIR_RUSSIA = 0x046D;
        public const ushort SUBLANG_BASQUE_BASQUE = 0x042D;
        public const ushort SUBLANG_BELARUSIAN_BELARUS = 0x0423;
        public const ushort SUBLANG_BOSNIAN_BOSNIA_HERZEGOVINA_CYRILLIC = 0x201A;
        public const ushort SUBLANG_BOSNIAN_BOSNIA_HERZEGOVINA_LATIN = 0x141A;
        public const ushort SUBLANG_BRETON_FRANCE = 0x047E;
        public const ushort SUBLANG_BULGARIAN_BULGARIA = 0x0402;
        public const ushort SUBLANG_CENTRAL_KURDISH_IRAQ = 0x0492;
        public const ushort SUBLANG_CHEROKEE_CHEROKEE = 0x045C;
        public const ushort SUBLANG_CATALAN_CATALAN = 0x0403;
        public const ushort SUBLANG_CHINESE_HONGKONG = 0x0C04;
        public const ushort SUBLANG_CHINESE_MACAU = 0x1404;
        public const ushort SUBLANG_CHINESE_SINGAPORE = 0x1004;
        public const ushort SUBLANG_CHINESE_SIMPLIFIED = 0x0004;
        public const ushort SUBLANG_CHINESE_TRADITIONAL = 0x7C04;
        public const ushort SUBLANG_CORSICAN_FRANCE = 0x0483;
        public const ushort SUBLANG_CROATIAN_BOSNIA_HERZEGOVINA_LATIN = 0x101A;
        public const ushort SUBLANG_CROATIAN_CROATIA = 0x041A;
        public const ushort SUBLANG_CZECH_CZECH_REPUBLIC = 0x0405;
        public const ushort SUBLANG_DANISH_DENMARK = 0x0406;
        public const ushort SUBLANG_DARI_AFGHANISTAN = 0x048C;
        public const ushort SUBLANG_DIVEHI_MALDIVES = 0x0465;
        public const ushort SUBLANG_DUTCH_BELGIAN = 0x0813;
        public const ushort SUBLANG_DUTCH = 0x0413;
        public const ushort SUBLANG_ENGLISH_AUS = 0x0C09;
        public const ushort SUBLANG_ENGLISH_BELIZE = 0x2809;
        public const ushort SUBLANG_ENGLISH_CAN = 0x1009;
        public const ushort SUBLANG_ENGLISH_CARIBBEAN = 0x2409;
        public const ushort SUBLANG_ENGLISH_INDIA = 0x4009;
        public const ushort SUBLANG_ENGLISH_EIRE = 0x1809;
        public const ushort SUBLANG_ENGLISH_IRELAND = 0x1809;
        public const ushort SUBLANG_ENGLISH_JAMAICA = 0x2009;
        public const ushort SUBLANG_ENGLISH_MALAYSIA = 0x4409;
        public const ushort SUBLANG_ENGLISH_NZ = 0x1409;
        public const ushort SUBLANG_ENGLISH_PHILIPPINES = 0x3409;
        public const ushort SUBLANG_ENGLISH_SINGAPORE = 0x4809;
        public const ushort SUBLANG_ENGLISH_SOUTH_AFRICA = 0x1C09;
        public const ushort SUBLANG_ENGLISH_TRINIDAD = 0x2C09;
        public const ushort SUBLANG_ENGLISH_UK = 0x0809;
        public const ushort SUBLANG_ENGLISH_US = 0x0409;
        public const ushort SUBLANG_ENGLISH_ZIMBABWE = 0x3009;
        public const ushort SUBLANG_ESTONIAN_ESTONIA = 0x0425;
        public const ushort SUBLANG_FAEROESE_FAROE_ISLANDS = 0x0438;
        public const ushort SUBLANG_FILIPINO_PHILIPPINES = 0x0464;
        public const ushort SUBLANG_FINNISH_FINLAND = 0x040B;
        public const ushort SUBLANG_FRENCH_BELGIAN = 0x080C;
        public const ushort SUBLANG_FRENCH_CANADIAN = 0x0C0C;
        public const ushort SUBLANG_FRENCH = 0x040C;
        public const ushort SUBLANG_FRENCH_LUXEMBOURG = 0x140C;
        public const ushort SUBLANG_FRENCH_MONACO = 0x180C;
        public const ushort SUBLANG_FRENCH_SWISS = 0x100C;
        public const ushort SUBLANG_FRISIAN_NETHERLANDS = 0x0462;
        public const ushort SUBLANG_GALICIAN_GALICIAN = 0x0456;
        public const ushort SUBLANG_GEORGIAN_GEORGIA = 0x0437;
        public const ushort SUBLANG_GERMAN_AUSTRIAN = 0x0C07;
        public const ushort SUBLANG_GERMAN = 0x0407;
        public const ushort SUBLANG_GERMAN_LIECHTENSTEIN = 0x1407;
        public const ushort SUBLANG_GERMAN_LUXEMBOURG = 0x1007;
        public const ushort SUBLANG_GERMAN_SWISS = 0x0807;
        public const ushort SUBLANG_GREEK_GREECE = 0x0408;
        public const ushort SUBLANG_GREENLANDIC_GREENLAND = 0x046F;
        public const ushort SUBLANG_GUJARATI_INDIA = 0x0447;
        public const ushort SUBLANG_HAUSA_NIGERIA_LATIN = 0x0468;
        public const ushort SUBLANG_HAWAIIAN_US = 0x0475;
        public const ushort SUBLANG_HEBREW_ISRAEL = 0x040D;
        public const ushort SUBLANG_HINDI_INDIA = 0x0439;
        public const ushort SUBLANG_HUNGARIAN_HUNGARY = 0x040E;
        public const ushort SUBLANG_ICELANDIC_ICELAND = 0x040F;
        public const ushort SUBLANG_IGBO_NIGERIA = 0x0470;
        public const ushort SUBLANG_INDONESIAN_INDONESIA = 0x0421;
        public const ushort SUBLANG_INUKTITUT_CANADA_LATIN = 0x085D;
        public const ushort SUBLANG_INUKTITUT_CANADA = 0x045D;
        public const ushort SUBLANG_IRISH_IRELAND = 0x083C;
        public const ushort SUBLANG_XHOSA_SOUTH_AFRICA = 0x0434;
        public const ushort SUBLANG_ZULU_SOUTH_AFRICA = 0x0435;
        public const ushort SUBLANG_ITALIAN = 0x0410;
        public const ushort SUBLANG_ITALIAN_SWISS = 0x0810;
        public const ushort SUBLANG_JAPANESE_JAPAN = 0x0411;
        public const ushort SUBLANG_KANNADA_INDIA = 0x044B;
        public const ushort SUBLANG_KAZAK_KAZAKHSTAN = 0x043F;
        public const ushort SUBLANG_KHMER_CAMBODIA = 0x0453;
        public const ushort SUBLANG_KICHE_GUATEMALA = 0x0486;
        public const ushort SUBLANG_KINYARWANDA_RWANDA = 0x0487;
        public const ushort SUBLANG_KONKANI_INDIA = 0x0457;
        public const ushort SUBLANG_KOREAN = 0x0412;
        public const ushort SUBLANG_KYRGYZ_KYRGYZSTAN = 0x0440;
        public const ushort SUBLANG_LAO_LAO = 0x0454;
        public const ushort SUBLANG_LATVIAN_LATVIA = 0x0426;
        public const ushort SUBLANG_LITHUANIAN_LITHUANIA = 0x0427;
        public const ushort SUBLANG_LOWER_SORBIAN_GERMANY = 0x082E;
        public const ushort SUBLANG_LUXEMBOURGISH_LUXEMBOURG = 0x046E;
        public const ushort SUBLANG_MACEDONIAN_MACEDONIA = 0x042F;
        public const ushort SUBLANG_MALAY_BRUNEI_DARUSSALAM = 0x083E;
        public const ushort SUBLANG_MALAY_MALAYSIA = 0x043E;
        public const ushort SUBLANG_MALAYALAM_INDIA = 0x044C;
        public const ushort SUBLANG_MALTESE_MALTA = 0x043A;
        public const ushort SUBLANG_MAORI_NEW_ZEALAND = 0x0481;
        public const ushort SUBLANG_MAPUDUNGUN_CHILE = 0x047A;
        public const ushort SUBLANG_MARATHI_INDIA = 0x044E;
        public const ushort SUBLANG_MOHAWK_MOHAWK = 0x047C;
        public const ushort SUBLANG_MONGOLIAN_CYRILLIC_MONGOLIA = 0x0450;
        public const ushort SUBLANG_MONGOLIAN_PRC = 0x0850;
        public const ushort SUBLANG_NEPALI_NEPAL = 0x0461;
        public const ushort SUBLANG_NORWEGIAN_BOKMAL = 0x0414;
        public const ushort SUBLANG_NORWEGIAN_NYNORSK = 0x0814;
        public const ushort SUBLANG_OCCITAN_FRANCE = 0x0482;
        public const ushort SUBLANG_ORIYA_INDIA = 0x0448;
        public const ushort SUBLANG_PASHTO_AFGHANISTAN = 0x0463;
        public const ushort SUBLANG_PERSIAN_IRAN = 0x0429;
        public const ushort SUBLANG_POLISH_POLAND = 0x0415;
        public const ushort SUBLANG_PORTUGUESE_BRAZILIAN = 0x0416;
        public const ushort SUBLANG_PORTUGUESE = 0x0816;
        public const ushort SUBLANG_PULAR_SENEGAL = 0x0867;
        public const ushort SUBLANG_PUNJABI_INDIA = 0x0446;
        public const ushort SUBLANG_PUNJABI_PAKISTAN = 0x0846;
        public const ushort SUBLANG_QUECHUA_BOLIVIA = 0x046B;
        public const ushort SUBLANG_QUECHUA_ECUADOR = 0x086B;
        public const ushort SUBLANG_QUECHUA_PERU = 0x0C6B;
        public const ushort SUBLANG_ROMANIAN_ROMANIA = 0x0418;
        public const ushort SUBLANG_ROMANSH_SWITZERLAND = 0x0417;
        public const ushort SUBLANG_RUSSIAN_RUSSIA = 0x0419;
        public const ushort SUBLANG_SAKHA_RUSSIA = 0x0485;
        public const ushort SUBLANG_SAMI_INARI_FINLAND = 0x243B;
        public const ushort SUBLANG_SAMI_LULE_NORWAY = 0x103B;
        public const ushort SUBLANG_SAMI_LULE_SWEDEN = 0x143B;
        public const ushort SUBLANG_SAMI_NORTHERN_FINLAND = 0x0C3B;
        public const ushort SUBLANG_SAMI_NORTHERN_NORWAY = 0x043B;
        public const ushort SUBLANG_SAMI_NORTHERN_SWEDEN = 0x083B;
        public const ushort SUBLANG_SAMI_SKOLT_FINLAND = 0x203B;
        public const ushort SUBLANG_SAMI_SOUTHERN_NORWAY = 0x183B;
        public const ushort SUBLANG_SAMI_SOUTHERN_SWEDEN = 0x1C3B;
        public const ushort SUBLANG_SANSKRIT_INDIA = 0x044F;
        public const ushort SUBLANG_SERBIAN_BOSNIA_HERZEGOVINA_CYRILLIC = 0x1C1A;
        public const ushort SUBLANG_SERBIAN_BOSNIA_HERZEGOVINA_LATIN = 0x181A;
        public const ushort SUBLANG_SERBIAN_CYRILLIC = 0x0C1A;
        public const ushort SUBLANG_SERBIAN_LATIN = 0x081A;
        public const ushort SUBLANG_SOTHO_NORTHERN_SOUTH_AFRICA = 0x046C;
        public const ushort SUBLANG_TSWANA_BOTSWANA = 0x0832;
        public const ushort SUBLANG_TSWANA_SOUTH_AFRICA = 0x0432;
        public const ushort SUBLANG_SINDHI_INDIA = 0x0459;
        public const ushort SUBLANG_SINDHI_PAKISTAN = 0x0859;
        public const ushort SUBLANG_SINHALESE_SRI_LANKA = 0x045B;
        public const ushort SUBLANG_SLOVAK_SLOVAKIA = 0x041B;
        public const ushort SUBLANG_SLOVENIAN_SLOVENIA = 0x0424;
        public const ushort SUBLANG_SPANISH_ARGENTINA = 0x2C0A;
        public const ushort SUBLANG_SPANISH_BOLIVIA = 0x400A;
        public const ushort SUBLANG_SPANISH_CHILE = 0x340A;
        public const ushort SUBLANG_SPANISH_COLOMBIA = 0x240A;
        public const ushort SUBLANG_SPANISH_COSTA_RICA = 0x140A;
        public const ushort SUBLANG_SPANISH_DOMINICAN_REPUBLIC = 0x1C0A;
        public const ushort SUBLANG_SPANISH_ECUADOR = 0x300A;
        public const ushort SUBLANG_SPANISH_EL_SALVADOR = 0x440A;
        public const ushort SUBLANG_SPANISH_GUATEMALA = 0x100A;
        public const ushort SUBLANG_SPANISH_HONDURAS = 0x480A;
        public const ushort SUBLANG_SPANISH_MEXICAN = 0x080A;
        public const ushort SUBLANG_SPANISH_NICARAGUA = 0x4C0A;
        public const ushort SUBLANG_SPANISH_PANAMA = 0x180A;
        public const ushort SUBLANG_SPANISH_PARAGUAY = 0x3C0A;
        public const ushort SUBLANG_SPANISH_PERU = 0x280A;
        public const ushort SUBLANG_SPANISH_PUERTO_RICO = 0x500A;
        public const ushort SUBLANG_SPANISH_MODERN = 0x0C0A;
        public const ushort SUBLANG_SPANISH = 0x040A;
        public const ushort SUBLANG_SPANISH_US = 0x540A;
        public const ushort SUBLANG_SPANISH_URUGUAY = 0x380A;
        public const ushort SUBLANG_SPANISH_VENEZUELA = 0x200A;
        public const ushort SUBLANG_SWAHILI = 0x0441;
        public const ushort SUBLANG_SWEDISH_FINLAND = 0x081D;
        public const ushort SUBLANG_SWEDISH = 0x041D;
        public const ushort SUBLANG_SWEDISH_SWEDEN = 0x041D;
        public const ushort SUBLANG_SYRIAC = 0x045A;
        public const ushort SUBLANG_TAJIK_TAJIKISTAN = 0x0428;
        public const ushort SUBLANG_TAMAZIGHT_ALGERIA_LATIN = 0x085F;
        public const ushort SUBLANG_TAMIL_INDIA = 0x0449;
        public const ushort SUBLANG_TAMIL_SRI_LANKA = 0x0849;
        public const ushort SUBLANG_TATAR_RUSSIA = 0x0444;
        public const ushort SUBLANG_TELUGU_INDIA = 0x044A;
        public const ushort SUBLANG_THAI_THAILAND = 0x041E;
        public const ushort SUBLANG_TIBETAN_PRC = 0x0451;
        public const ushort SUBLANG_TIGRINYA_ERITREA = 0x0873;
        public const ushort SUBLANG_TIGRINYA_ETHIOPIA = 0x0473;
        public const ushort SUBLANG_TIGRIGNA_ERITREA = 0x0873;
        public const ushort SUBLANG_TURKISH_TURKEY = 0x041F;
        public const ushort SUBLANG_TURKMEN_TURKMENISTAN = 0x0442;
        public const ushort SUBLANG_UKRAINIAN_UKRAINE = 0x0422;
        public const ushort SUBLANG_UPPER_SORBIAN_GERMANY = 0x042E;
        public const ushort SUBLANG_URDU_INDIA = 0x0820;
        public const ushort SUBLANG_URDU_PAKISTAN = 0x0420;
        public const ushort SUBLANG_UIGHUR_PRC = 0x0480;
        public const ushort SUBLANG_UZBEK_CYRILLIC = 0x0843;
        public const ushort SUBLANG_UZBEK_LATIN = 0x0443;
        public const ushort SUBLANG_VALENCIAN_VALENCIA = 0x0803;
        public const ushort SUBLANG_VIETNAMESE_VIETNAM = 0x042A;
        public const ushort SUBLANG_WELSH_UNITED_KINGDOM = 0x0452;
        public const ushort SUBLANG_WOLOF_SENEGAL = 0x0488;
        public const ushort SUBLANG_YI_PRC = 0x0478;
        public const ushort SUBLANG_YORUBA_NIGERIA = 0x046A;

    }

}
