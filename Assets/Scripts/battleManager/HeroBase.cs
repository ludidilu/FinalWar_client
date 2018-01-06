using UnityEngine;
using textureFactory;
using System;

public class HeroBase : MonoBehaviour
{
    public HeroSDS sds { get; protected set; }

    public int cardUid { get; protected set; }

    protected void InitCard(HeroSDS _heroSDS)
    {
        sds = _heroSDS;

        TextureFactory.Instance.GetTexture<Sprite>("Assets/Resource/texture/" + sds.heroTypeFix.icon + ".png", GetHeroTypeSprite, true);

        TextureFactory.Instance.GetTexture<Sprite>("Assets/Resource/texture/" + sds.icon + ".png", GetBodySprite, true);
    }

    protected virtual void GetHeroTypeSprite(Sprite _sp)
    {
        throw new NotImplementedException();
    }

    protected virtual void GetBodySprite(Sprite _sp)
    {
        throw new NotImplementedException();
    }
}
