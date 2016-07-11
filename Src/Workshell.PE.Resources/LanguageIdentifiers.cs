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

    public sealed class LanguageIdentifiers
    {

        public const byte LANG_NEUTRAL                         = 0x00;
        public const byte LANG_INVARIANT                       = 0x7f;
        public const byte LANG_AFRIKAANS                       = 0x36;
        public const byte LANG_ALBANIAN                        = 0x1c;
        public const byte LANG_ARABIC                          = 0x01;
        public const byte LANG_BASQUE                          = 0x2d;
        public const byte LANG_BELARUSIAN                      = 0x23;
        public const byte LANG_BULGARIAN                       = 0x02;
        public const byte LANG_CATALAN                         = 0x03;
        public const byte LANG_CHINESE                         = 0x04;
        public const byte LANG_CROATIAN                        = 0x1a;
        public const byte LANG_CZECH                           = 0x05;
        public const byte LANG_DANISH                          = 0x06;
        public const byte LANG_DUTCH                           = 0x13;
        public const byte LANG_ENGLISH                         = 0x09;
        public const byte LANG_ESTONIAN                        = 0x25;
        public const byte LANG_FAEROESE                        = 0x38;
        public const byte LANG_FARSI                           = 0x29;
        public const byte LANG_FINNISH                         = 0x0b;
        public const byte LANG_FRENCH                          = 0x0c;
        public const byte LANG_GERMAN                          = 0x07;
        public const byte LANG_GREEK                           = 0x08;
        public const byte LANG_HEBREW                          = 0x0d;
        public const byte LANG_HUNGARIAN                       = 0x0e;
        public const byte LANG_ICELANDIC                       = 0x0f;
        public const byte LANG_INDONESIAN                      = 0x21;
        public const byte LANG_ITALIAN                         = 0x10;
        public const byte LANG_JAPANESE                        = 0x11;
        public const byte LANG_KOREAN                          = 0x12;
        public const byte LANG_LATVIAN                         = 0x26;
        public const byte LANG_LITHUANIAN                      = 0x27;
        public const byte LANG_NORWEGIAN                       = 0x14;
        public const byte LANG_POLISH                          = 0x15;
        public const byte LANG_PORTUGUESE                      = 0x16;
        public const byte LANG_ROMANIAN                        = 0x18;
        public const byte LANG_RUSSIAN                         = 0x19;
        public const byte LANG_SERBIAN                         = 0x1a;
        public const byte LANG_SLOVAK                          = 0x1b;
        public const byte LANG_SLOVENIAN                       = 0x24;
        public const byte LANG_SPANISH                         = 0x0a;
        public const byte LANG_SWEDISH                         = 0x1d;
        public const byte LANG_THAI                            = 0x1e;
        public const byte LANG_TURKISH                         = 0x1f;
        public const byte LANG_UKRAINIAN                       = 0x22;
        public const byte LANG_VIETNAMESE                      = 0x2a;

        public const byte SUBLANG_NEUTRAL                      = 0x00;    // language neutral 
        public const byte SUBLANG_DEFAULT                      = 0x01;    // user default 
        public const byte SUBLANG_SYS_DEFAULT                  = 0x02;    // system default 
        public const byte SUBLANG_CUSTOM_UNSPECIFIED           = 0x04;    // custom language/locale 
        public const byte SUBLANG_UI_CUSTOM_DEFAULT            = 0x05;    // Default custom MUI language/locale 

        public const byte SUBLANG_ARABIC_SAUDI_ARABIA          = 0x01;    // Arabic (Saudi Arabia) 
        public const byte SUBLANG_ARABIC_IRAQ                  = 0x02;    // Arabic (Iraq) 
        public const byte SUBLANG_ARABIC_EGYPT                 = 0x03;    // Arabic (Egypt) 
        public const byte SUBLANG_ARABIC_LIBYA                 = 0x04;    // Arabic (Libya) 
        public const byte SUBLANG_ARABIC_ALGERIA               = 0x05;    // Arabic (Algeria) 
        public const byte SUBLANG_ARABIC_MOROCCO               = 0x06;    // Arabic (Morocco) 
        public const byte SUBLANG_ARABIC_TUNISIA               = 0x07;    // Arabic (Tunisia) 
        public const byte SUBLANG_ARABIC_OMAN                  = 0x08;    // Arabic (Oman) 
        public const byte SUBLANG_ARABIC_YEMEN                 = 0x09;    // Arabic (Yemen) 
        public const byte SUBLANG_ARABIC_SYRIA                 = 0x0a;    // Arabic (Syria) 
        public const byte SUBLANG_ARABIC_JORDAN                = 0x0b;    // Arabic (Jordan) 
        public const byte SUBLANG_ARABIC_LEBANON               = 0x0c;    // Arabic (Lebanon) 
        public const byte SUBLANG_ARABIC_KUWAIT                = 0x0d;    // Arabic (Kuwait) 
        public const byte SUBLANG_ARABIC_UAE                   = 0x0e;    // Arabic (U.A.E) 
        public const byte SUBLANG_ARABIC_BAHRAIN               = 0x0f;    // Arabic (Bahrain) 
        public const byte SUBLANG_ARABIC_QATAR                 = 0x10;    // Arabic (Qatar) 

        public const byte SUBLANG_CHINESE_TRADITIONAL          = 0x01;    // Chinese (Taiwan) 
        public const byte SUBLANG_CHINESE_SIMPLIFIED           = 0x02;    // Chinese (PR China) 
        public const byte SUBLANG_CHINESE_HONGKONG             = 0x03;    // Chinese (Hong Kong) 
        public const byte SUBLANG_CHINESE_SINGAPORE            = 0x04;    // Chinese (Singapore) 

        public const byte SUBLANG_DUTCH                        = 0x01;    // Dutch 
        public const byte SUBLANG_DUTCH_BELGIAN                = 0x02;    // Dutch (Belgian) 

        public const byte SUBLANG_ENGLISH_US                   = 0x01;    // English (USA) 
        public const byte SUBLANG_ENGLISH_UK                   = 0x02;    // English (UK) 
        public const byte SUBLANG_ENGLISH_AUS                  = 0x03;    // English (Australian) 
        public const byte SUBLANG_ENGLISH_CAN                  = 0x04;    // English (Canadian) 
        public const byte SUBLANG_ENGLISH_NZ                   = 0x05;    // English (New Zealand) 
        public const byte SUBLANG_ENGLISH_EIRE                 = 0x06;    // English (Irish) 
        public const byte SUBLANG_ENGLISH_SOUTH_AFRICA         = 0x07;    // English (South Africa) 
        public const byte SUBLANG_ENGLISH_JAMAICA              = 0x08;    // English (Jamaica) 
        public const byte SUBLANG_ENGLISH_CARIBBEAN            = 0x09;    // English (Caribbean) 
        public const byte SUBLANG_ENGLISH_BELIZE               = 0x0a;    // English (Belize) 
        public const byte SUBLANG_ENGLISH_TRINIDAD             = 0x0b;    // English (Trinidad) 

        public const byte SUBLANG_FRENCH                       = 0x01;    // French 
        public const byte SUBLANG_FRENCH_BELGIAN               = 0x02;    // French (Belgian) 
        public const byte SUBLANG_FRENCH_CANADIAN              = 0x03;    // French (Canadian) 
        public const byte SUBLANG_FRENCH_SWISS                 = 0x04;    // French (Swiss) 
        public const byte SUBLANG_FRENCH_LUXEMBOURG            = 0x05;    // French (Luxembourg) 

        public const byte SUBLANG_GERMAN                       = 0x01;    // German 
        public const byte SUBLANG_GERMAN_SWISS                 = 0x02;    // German (Swiss) 
        public const byte SUBLANG_GERMAN_AUSTRIAN              = 0x03;    // German (Austrian) 
        public const byte SUBLANG_GERMAN_LUXEMBOURG            = 0x04;    // German (Luxembourg) 
        public const byte SUBLANG_GERMAN_LIECHTENSTEIN         = 0x05;    // German (Liechtenstein) 

        public const byte SUBLANG_ITALIAN                      = 0x01;    // Italian 
        public const byte SUBLANG_ITALIAN_SWISS                = 0x02;    // Italian (Swiss) 

        public const byte SUBLANG_KOREAN                       = 0x01;    // Korean (Extended Wansung) 
        public const byte SUBLANG_KOREAN_JOHAB                 = 0x02;    // Korean (Johab) 

        public const byte SUBLANG_NORWEGIAN_BOKMAL             = 0x01;    // Norwegian (Bokmal) 
        public const byte SUBLANG_NORWEGIAN_NYNORSK            = 0x02;    // Norwegian (Nynorsk) 

        public const byte SUBLANG_PORTUGUESE                   = 0x02;    // Portuguese 
        public const byte SUBLANG_PORTUGUESE_BRAZILIAN         = 0x01;    // Portuguese (Brazilian) 

        public const byte SUBLANG_SERBIAN_LATIN                = 0x02;    // Serbian (Latin) 
        public const byte SUBLANG_SERBIAN_CYRILLIC             = 0x03;    // Serbian (Cyrillic) 

        public const byte SUBLANG_SPANISH                      = 0x01;    // Spanish (Castilian) 
        public const byte SUBLANG_SPANISH_MEXICAN              = 0x02;    // Spanish (Mexican) 
        public const byte SUBLANG_SPANISH_MODERN               = 0x03;    // Spanish (Modern) 
        public const byte SUBLANG_SPANISH_GUATEMALA            = 0x04;    // Spanish (Guatemala) 
        public const byte SUBLANG_SPANISH_COSTA_RICA           = 0x05;    // Spanish (Costa Rica) 
        public const byte SUBLANG_SPANISH_PANAMA               = 0x06;    // Spanish (Panama) 
        public const byte SUBLANG_SPANISH_DOMINICAN_REPUBLIC   = 0x07;    // Spanish (Dominican Republic) 
        public const byte SUBLANG_SPANISH_VENEZUELA            = 0x08;    // Spanish (Venezuela) 
        public const byte SUBLANG_SPANISH_COLOMBIA             = 0x09;    // Spanish (Colombia) 
        public const byte SUBLANG_SPANISH_PERU                 = 0x0a;    // Spanish (Peru) 
        public const byte SUBLANG_SPANISH_ARGENTINA            = 0x0b;    // Spanish (Argentina) 
        public const byte SUBLANG_SPANISH_ECUADOR              = 0x0c;    // Spanish (Ecuador) 
        public const byte SUBLANG_SPANISH_CHILE                = 0x0d;    // Spanish (Chile) 
        public const byte SUBLANG_SPANISH_URUGUAY              = 0x0e;    // Spanish (Uruguay) 
        public const byte SUBLANG_SPANISH_PARAGUAY             = 0x0f;    // Spanish (Paraguay) 
        public const byte SUBLANG_SPANISH_BOLIVIA              = 0x10;    // Spanish (Bolivia) 
        public const byte SUBLANG_SPANISH_EL_SALVADOR          = 0x11;    // Spanish (El Salvador) 
        public const byte SUBLANG_SPANISH_HONDURAS             = 0x12;    // Spanish (Honduras) 
        public const byte SUBLANG_SPANISH_NICARAGUA            = 0x13;    // Spanish (Nicaragua) 
        public const byte SUBLANG_SPANISH_PUERTO_RICO          = 0x14;    // Spanish (Puerto Rico) 

        public const byte SUBLANG_SWEDISH                      = 0x01;    // Swedish 
        public const byte SUBLANG_SWEDISH_FINLAND              = 0x02;    // Swedish (Finland) 

    }

}
