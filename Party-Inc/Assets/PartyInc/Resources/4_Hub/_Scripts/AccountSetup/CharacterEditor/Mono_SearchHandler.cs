using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_SearchHandler : MonoBehaviour
        {
            [SerializeField] private Mono_ModalScreen _searchModal;
            [SerializeField] private Text _searchTextBox;
            [SerializeField] private GameObject _searchButtonGO;
            [SerializeField] private GameObject _cancelSearchButtonGO;

            private string _nameToSearch;

            public void ApplySearch()
            {
                AssetFilterSettings aux = Mng_CharacterEditorCache.Current.CurrentFilter;
                aux.name = _searchTextBox.text;
                Mng_CharacterEditorCache.Current.CurrentFilter = aux;

                _searchModal.OnCloseModal();

                _searchButtonGO.SetActive(false);
                _cancelSearchButtonGO.SetActive(true);

                Mng_Reloader.Current.Reload();
            }

            public void UndoSearch()
            {
                AssetFilterSettings aux = Mng_CharacterEditorCache.Current.CurrentFilter;
                aux.name = "";
                Mng_CharacterEditorCache.Current.CurrentFilter = aux;

                _searchButtonGO.SetActive(true);
                _cancelSearchButtonGO.SetActive(false);

                Mng_Reloader.Current.Reload();
            }
        }
    }
}


