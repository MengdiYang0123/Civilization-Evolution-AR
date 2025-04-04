using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WPM {

    public class EditorAttribGroup {
        public IExtendableAttribute itemGroup;
        public string newTagKey;
        public string newTagValue;
        public bool foldOut;

        public void SetItemGroup(IExtendableAttribute item) {
            itemGroup = item;
            newTagKey = "";
            newTagValue = "";
            
        }
        
       /* public void GetItemGroup() {
            itemGroup = item;
            
        }*/
        
        

    }

}
