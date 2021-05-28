using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_FilterHandler : MonoBehaviour
        {
            [SerializeField] private Mono_ModalScreen _filterModal;
            [SerializeField] private Toggle _priceHiToLowToggle;
            [SerializeField] private Toggle _priceLowToHiToggle;
            [SerializeField] private Toggle _releaseDateLaterToggle;
            [SerializeField] private Toggle _releaseDateRecentToggle;

            private string _nameToSearch;

            public void ApplyFilter()
            {
                AssetFilterSettings aux = Mng_CharacterEditorCache.Current.CurrentFilter;
                aux.maximumPrice = 0;
                aux.minimumPrice = 0;
                aux.priceHiToLow = _priceHiToLowToggle.isOn;
                aux.priceLowToHi = _priceLowToHiToggle.isOn;
                aux.releaseDateLater = _releaseDateLaterToggle.isOn;
                aux.releaseDateRecent = _releaseDateRecentToggle.isOn;
                Mng_CharacterEditorCache.Current.CurrentFilter = aux;

                _filterModal.OnCloseModal();

                Mng_Reloader.Current.Reload();
            }

            public void UndoFilter()
            {
                AssetFilterSettings aux = Mng_CharacterEditorCache.Current.CurrentFilter;
                aux.maximumPrice = 0;
                aux.minimumPrice = 0;
                aux.priceHiToLow = false;
                aux.priceLowToHi = false;
                aux.releaseDateLater = false;
                aux.releaseDateRecent = false;
                Mng_CharacterEditorCache.Current.CurrentFilter = aux;

                Mng_Reloader.Current.Reload();
            }
        }
    }
}


