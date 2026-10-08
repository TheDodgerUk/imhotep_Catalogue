using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BrandData
{
    public List<BrandInfo> BrandInfo = new List<BrandInfo>();
}

[Serializable]
public class BrandInfo
{
    public string BrandName;
    public int EstYear;
    public string CountryLocation;
    public string CityLocation;
    public string BrandDescription;

    public string BrandVideoName;
    public List<string> BrandVideos = new List<string>();
}
