﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace PokeFinder.Models
{
    public static class PokemonList
    {
        private static string pokemonIdString = @"{
    'BULBASAUR': 1,
    'IVYSAUR': 2,
    'VENUSAUR': 3,
    'CHARMANDER': 4,
    'CHARMELEON': 5,
    'CHARIZARD': 6,
    'SQUIRTLE': 7,
    'WARTORTLE': 8,
    'BLASTOISE': 9,
    'CATERPIE': 10,
    'METAPOD': 11,
    'BUTTERFREE': 12,
    'WEEDLE': 13,
    'KAKUNA': 14,
    'BEEDRILL': 15,
    'PIDGEY': 16,
    'PIDGEOTTO': 17,
    'PIDGEOT': 18,
    'RATTATA': 19,
    'RATICATE': 20,
    'SPEAROW': 21,
    'FEAROW': 22,
    'EKANS': 23,
    'ARBOK': 24,
    'PIKACHU': 25,
    'RAICHU': 26,
    'SANDSHREW': 27,
    'SANDSLASH': 28,
    'NIDORAN_FEMALE': 29,
    'NIDORINA': 30,
    'NIDOQUEEN': 31,
    'NIDORAN_MALE': 32,
    'NIDORINO': 33,
    'NIDOKING': 34,
    'CLEFAIRY': 35,
    'CLEFABLE': 36,
    'VULPIX': 37,
    'NINETALES': 38,
    'JIGGLYPUFF': 39,
    'WIGGLYTUFF': 40,
    'ZUBAT': 41,
    'GOLBAT': 42,
    'ODDISH': 43,
    'GLOOM': 44,
    'VILEPLUME': 45,
    'PARAS': 46,
    'PARASECT': 47,
    'VENONAT': 48,
    'VENOMOTH': 49,
    'DIGLETT': 50,
    'DUGTRIO': 51,
    'MEOWTH': 52,
    'PERSIAN': 53,
    'PSYDUCK': 54,
    'GOLDUCK': 55,
    'MANKEY': 56,
    'PRIMEAPE': 57,
    'GROWLITHE': 58,
    'ARCANINE': 59,
    'POLIWAG': 60,
    'POLIWHIRL': 61,
    'POLIWRATH': 62,
    'ABRA': 63,
    'KADABRA': 64,
    'ALAKAZAM': 65,
    'MACHOP': 66,
    'MACHOKE': 67,
    'MACHAMP': 68,
    'BELLSPROUT': 69,
    'WEEPINBELL': 70,
    'VICTREEBEL': 71,
    'TENTACOOL': 72,
    'TENTACRUEL': 73,
    'GEODUDE': 74,
    'GRAVELER': 75,
    'GOLEM': 76,
    'PONYTA': 77,
    'RAPIDASH': 78,
    'SLOWPOKE': 79,
    'SLOWBRO': 80,
    'MAGNEMITE': 81,
    'MAGNETON': 82,
    'FARFETCHD': 83,
    'DODUO': 84,
    'DODRIO': 85,
    'SEEL': 86,
    'DEWGONG': 87,
    'GRIMER': 88,
    'MUK': 89,
    'SHELLDER': 90,
    'CLOYSTER': 91,
    'GASTLY': 92,
    'HAUNTER': 93,
    'GENGAR': 94,
    'ONIX': 95,
    'DROWZEE': 96,
    'HYPNO': 97,
    'KRABBY': 98,
    'KINGLER': 99,
    'VOLTORB': 100,
    'ELECTRODE': 101,
    'EXEGGCUTE': 102,
    'EXEGGUTOR': 103,
    'CUBONE': 104,
    'MAROWAK': 105,
    'HITMONLEE': 106,
    'HITMONCHAN': 107,
    'LICKITUNG': 108,
    'KOFFING': 109,
    'WEEZING': 110,
    'RHYHORN': 111,
    'RHYDON': 112,
    'CHANSEY': 113,
    'TANGELA': 114,
    'KANGASKHAN': 115,
    'HORSEA': 116,
    'SEADRA': 117,
    'GOLDEEN': 118,
    'SEAKING': 119,
    'STARYU': 120,
    'STARMIE': 121,
    'MR_MIME': 122,
    'SCYTHER': 123,
    'JYNX': 124,
    'ELECTABUZZ': 125,
    'MAGMAR': 126,
    'PINSIR': 127,
    'TAUROS': 128,
    'MAGIKARP': 129,
    'GYARADOS': 130,
    'LAPRAS': 131,
    'DITTO': 132,
    'EEVEE': 133,
    'VAPOREON': 134,
    'JOLTEON': 135,
    'FLAREON': 136,
    'PORYGON': 137,
    'OMANYTE': 138,
    'OMASTAR': 139,
    'KABUTO': 140,
    'KABUTOPS': 141,
    'AERODACTYL': 142,
    'SNORLAX': 143,
    'ARTICUNO': 144,
    'ZAPDOS': 145,
    'MOLTRES': 146,
    'DRATINI': 147,
    'DRAGONAIR': 148,
    'DRAGONITE': 149,
    'MEWTWO': 150,
    'MEW': 151
}";

        public static Dictionary<int, string> PokemonPng;

        public static Dictionary<string, int> GetPokemonIdForNameDictionary() {
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(pokemonIdString);
        }

        public static void InitList() {
            if (PokemonPng == null)
            {
                PokemonPng = new Dictionary<int, string>();
                var pokemons = new HttpClient().GetStringAsync("https://gist.githubusercontent.com/anonymous/50c284e815df6c81aa53497a305a29f2/raw").Result.Split('\n');
                foreach (string t in pokemons)
                {
                    var data = t.Split(':');
                    if (data.Length == 2)
                    {
                        PokemonPng.Add(Convert.ToInt32(data[0]), data[1]);
                    }
                }
            }
        }

        public static string GetPngForPokemonId(int id) {
            InitList();
            return PokemonPng[id];
        }
    }
}