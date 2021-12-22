using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils
{
    //public static Sprite loadingSprite;
     public static void SetSprite(Image image,string path)
    {
        //if (loadingSprite == null)
        //{
        //    loadingSprite = Resources.Load<Sprite>("loading");
        //}
        //if(image.sprite = null)
        //{
        //    image.sprite = loadingSprite;
        //}
        ResLoader.Instance.GetUISprite(path, (sp) => {
            image.sprite = sp;
        });
    }

}
