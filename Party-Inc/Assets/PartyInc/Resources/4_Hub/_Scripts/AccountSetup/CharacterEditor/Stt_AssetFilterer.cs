using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace PartyInc
{
    namespace Hub
    {
        public class Stt_AssetFilterer
        {
            public static List<Data_CharacterAssetMetadata> FilterAssets(List<Data_CharacterAssetMetadata> assets, AssetFilterSettings settings)
            {
                List<Data_CharacterAssetMetadata> result = assets;

                result = FilterByName(result, settings.name);

                result = FilterByPriceRange(result, settings.minimumPrice, settings.maximumPrice);

                result = OrderByCriteria(result, settings);

                return result;
            }

            private static List<Data_CharacterAssetMetadata> FilterByName(List<Data_CharacterAssetMetadata> assets, string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return assets;
                }


                List<Data_CharacterAssetMetadata> result = assets;
                List<Data_CharacterAssetMetadata> aux = assets.ToList();

                foreach (Data_CharacterAssetMetadata asset in aux)
                {
                    AssetsStoreData assetData = Mng_CharacterEditorCache.Current.GetAssetStoreData(asset.AssetId, (int)asset.AssetType);

                    if (!assetData.storename.Contains(name))
                    {
                        result.Remove(asset);
                    }
                }

                return result;
            }

            private static List<Data_CharacterAssetMetadata> FilterByPriceRange(List<Data_CharacterAssetMetadata> assets, int min, int max)
            {
                if (min == 0 && max == 0)
                {
                    return assets;
                }

                List<Data_CharacterAssetMetadata> result = assets;

                foreach (Data_CharacterAssetMetadata asset in result)
                {
                    AssetsStoreData assetData = Mng_CharacterEditorCache.Current.GetAssetStoreData(asset.AssetId, (int)asset.AssetType);

                    if (assetData.baseprice < min || assetData.baseprice > max)
                    {
                        result.Remove(asset);
                    }
                }

                return result;
            }

            private static List<Data_CharacterAssetMetadata> OrderByCriteria(List<Data_CharacterAssetMetadata> assets, AssetFilterSettings settings)
            {
                if (settings.priceHiToLow)
                {
                    var res = assets.OrderByDescending(ass => Mng_CharacterEditorCache.Current.GetAssetStoreData(ass.AssetId, (int)ass.AssetType).baseprice);

                    return res.ToList();
                }

                if (settings.priceLowToHi)
                {
                    var res = assets.OrderBy(ass => Mng_CharacterEditorCache.Current.GetAssetStoreData(ass.AssetId, (int)ass.AssetType).baseprice);

                    return res.ToList();
                }

                return assets;

                if (settings.releaseDateRecent)
                {

                }

                if (settings.releaseDateLater)
                {

                }
            }
        }

        public struct AssetFilterSettings
        {
            public string name;
            public bool priceHiToLow;
            public bool priceLowToHi;
            public bool releaseDateRecent;
            public bool releaseDateLater;
            public int minimumPrice;
            public int maximumPrice;

            public AssetFilterSettings(string name, bool priceHiToLow, bool priceLowToHi, bool releaseDateRecent, bool releaseDateLater, int minimumPrice, int maximumPrice)
            {
                this.name = name;
                this.priceHiToLow = priceHiToLow;
                this.priceLowToHi = priceLowToHi;
                this.releaseDateRecent = releaseDateRecent;
                this.releaseDateLater = releaseDateLater;
                this.minimumPrice = minimumPrice;
                this.maximumPrice = maximumPrice;
            }
        }
    }
}


