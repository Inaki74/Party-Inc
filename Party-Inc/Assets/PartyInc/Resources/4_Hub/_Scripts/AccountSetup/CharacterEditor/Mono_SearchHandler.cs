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
            [SerializeField] private Button _searchButton;
            [SerializeField] private Image _searchButtonImage;

            [SerializeField] private Sprite _startSearch;
            [SerializeField] private Sprite _cancelSearch;

            private string _nameToSearch;

            public void ApplySearch()
            {
                AssetFilterSettings aux = Mng_CharacterEditorCache.Current.CurrentFilter;
                aux.name = _searchTextBox.text;
                Mng_CharacterEditorCache.Current.CurrentFilter = aux;

                _searchModal.OnCloseModal();

                _searchButtonImage.sprite = _cancelSearch;

                Mng_Reloader.Current.Reload();
            }

            public void UndoSearch()
            {
                AssetFilterSettings aux = Mng_CharacterEditorCache.Current.CurrentFilter;
                aux.name = "";
                Mng_CharacterEditorCache.Current.CurrentFilter = aux;

                _searchButtonImage.sprite = _startSearch;

                Mng_Reloader.Current.Reload();
            }
        }
    }
}


