﻿///**************************************************************
// * (c) Jens Richter 2014
// *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using KryptonEngine.Entities;
using System.IO;
using System.Xml.Serialization;


namespace KryptonEngine.Manager
{
	public class SpineDataManager : Manager<SpineData>
	{
		

	#region Singleton

	private static SpineDataManager mInstance;
	public static SpineDataManager Instance { get { if (mInstance == null) mInstance = new SpineDataManager(); return mInstance; } }

	#endregion

	#region Properties

	#endregion

	#region Constructor

	SpineDataManager()
	{
	}

	#endregion

	#region Methods

	public override void LoadContent()
	{
		//Setup Serializer
		XmlSerializer xml = new XmlSerializer(typeof(InteractiveObject));
		TextReader reader;
		//Goto SpineDirectory
		DirectoryInfo environmentPath = new DirectoryInfo(Environment.CurrentDirectory + @"\Content\spine\");
		if (!environmentPath.Exists) //Checken ob das Directory existiert
			return;
		foreach (FileInfo f in environmentPath.GetFiles()) //Enthaltene Files durchgehen
		{
			if (f.Name.EndsWith(".settings")) //.sData Files heraus filtern
			{
				reader = new StreamReader(f.FullName);
				SpineData.SpineDataSettings settings = (SpineData.SpineDataSettings)xml.Deserialize(reader); //sData File in SpineData Object umwandeln
				reader.Close();

				string TmpSkeletonName = f.Name.Remove(f.Name.Length - ".settings".Length);
				mRessourcen.Add(TmpSkeletonName, new SpineData(TmpSkeletonName, settings));
			}
		}
	}

	public override void Unload()
	{
		mRessourcen.Clear();
	}

	public override SpineData Add(string pName, string pPath)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Fügt ein neues Element in mRessourcenManager ein.
	/// </summary>
	/// <param name="pName">Name des Skeletons.</param>
	//public void Add(String pName)
	//{
	//	if (!mRessourcen.ContainsKey(pName))
	//	{
	//	SpineData data = new SpineData(pName);
	//	mRessourcen.Add(pName, data);
	//	}
	//}

	/// <summary>
	/// Gibt RawSpineData zurück.
	/// </summary>
	public override SpineData GetElementByString(string pElementName)
	{
		if (mRessourcen.ContainsKey(pElementName))
		return mRessourcen[pElementName];

		throw new ArgumentException("Element not found!");
	}

	public Skeleton NewSkeleton(string pName, float pScale)
	{
		SpineData TmpSpineData = this.GetElementByString(pName);
		TmpSpineData.json.Scale = TmpSpineData.settings.Scaling; //Set Scaling
		SkeletonData TmpSkeletonData = TmpSpineData.json.ReadSkeletonData(EngineSettings.DefaultPathSpine + pName + ".json"); //Apply Json with Scaling to get skelData
		return new Skeleton(TmpSkeletonData);
	}

	public AnimationState NewAnimationState(SkeletonData pSkeletonData)
	{
		AnimationStateData TmpAnimationStateData = new AnimationStateData(pSkeletonData);
		SetFadingSettings(TmpAnimationStateData);//Set mixing between animations
		return new AnimationState(TmpAnimationStateData);
	}

		#region SpineSettings

	/// <summary>
	/// Wendet alle zum Skeleton passenden AnimationMixes auf animationStateData an.
	/// </summary>
	private void SetFadingSettings(AnimationStateData pAnimationStateData)
	{
		List<SpineData.AnimationMix> AnimationFadingList = this.GetElementByString(pAnimationStateData.SkeletonData.Name).settings.AnimationFading;
		foreach (SpineData.AnimationMix animMix in this.GetElementByString(pAnimationStateData.SkeletonData.Name).settings.AnimationFading)
		{
			pAnimationStateData.SetMix(animMix.From, animMix.To, animMix.Fading);
		}
	}

		#endregion

	#endregion
	}
}
