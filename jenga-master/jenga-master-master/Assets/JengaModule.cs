using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JengaModule : MonoBehaviour
{
	public KMAudio mAudio;
	public KMBombModule BombModule;
	public KMSelectable[] JengaPieceSelectable;
	public JengaBlock[] jengaBlockCompact;
	public Sprite[] characters;

	int[] idxSticker1 = new int[20], idxSticker2 = new int[20];
	List<int> idxJengaToPull = new List<int>();
	static int curModID = 1;
	int loggindModID;
	bool[,] idxCanPull = new[,] {
        {true, false, false, true, false, true, true, false, true, false },
		{true, true, false, false, true, true, true, false, true, false },
		{false, false, false, true, false, false, false, true, true, true },
		{true, true, true, false, false, true, true, false, true, false },
		{true, false, false, false, true, false, false, true, false, false },
		{false, false, true, false, false, false, true, true, true, false },
		{true, true, true, true, false, true, false, true, false, true },
		{false, false, false, false, true, false, false, false, false, true },
		{true, false, true, false, false, true, true, true, false, true },
		{true, false, false, true, false, true, true, true, true, true },
	};

	void Start()
	{
		loggindModID = curModID++;
		for (var x = 0; x < JengaPieceSelectable.Length; x++)
        {
			int y = x;
			JengaPieceSelectable[x].OnInteract += delegate { HandleInteraction(y); return false; };
        }
		do
		{
			for (var x = 0; x < idxSticker1.Length; x++)
            {
				var idx1 = Random.Range(0, 10);
				var idx2 = Random.Range(0, 10);

				idxSticker1[x] = idx1;
				idxSticker2[x] = idx2;

				if (idxCanPull[idx2, idx1])
					idxJengaToPull.Add(x);
			}
		}
		while (idxJengaToPull.Count == 0);
		Debug.LogFormat("[Jenga #{0}] Jenga pieces are labeled from 1-20 starting with the pieces going bottom to top on odd layers, left to right on even layers, starting on the first layer.", loggindModID);
		for (var y = 0; y < jengaBlockCompact.Length; y++)
        {
			jengaBlockCompact[y].sticker1.sprite = characters[idxSticker1[y]];
            jengaBlockCompact[y].sticker2.sprite = characters[idxSticker2[y] + 10];

			Debug.LogFormat("[Jenga #{0}] Jenga piece no. {1} has the following stickers on the short faces: [sticker{2}] [sticker{3}]", loggindModID, y + 1, idxSticker1[y] + 1, idxSticker2[y] + 11);
			Debug.LogFormat("[Jenga #{0}] That piece should{1} be pulled.", loggindModID, idxJengaToPull.Contains(y) ? "" : " not");
		}

	}
	void HandleInteraction(int idx)
    {
		mAudio.PlaySoundAtTransform("WoodClick", jengaBlockCompact[idx].transform);
		if (jengaBlockCompact[idx].jengaBlock.activeSelf)
		{
			jengaBlockCompact[idx].jengaBlock.SetActive(false);
			if (idxJengaToPull.Contains(idx))
			{
				idxJengaToPull.Remove(idx);
				if (idxJengaToPull.Count <= 0)
					ModuleCompleted();
			}
			else if (idxJengaToPull.Count <= 0)
			{
				ModuleCompleted();
			}
			else
			{
				Debug.LogFormat("[Jenga #{0}] Strike! You pulled Jenga piece no. {1} which should not have been pulled.", loggindModID, idx + 1);
				ModuleFail();
			}
		}
    }

	protected bool ModuleFail()
	{
		if (idxJengaToPull.Count > 0)
			BombModule.HandleStrike();
		return false;
	}

	protected bool ModuleCompleted()
	{
		Debug.LogFormat("[Jenga #{0}] Module disarmed. All of the pieces that need to be pulled have been pulled.", loggindModID);
		BombModule.HandlePass();
		for (var x = 0; x < JengaPieceSelectable.Length; x++)
		{
			JengaPieceSelectable[x].OnInteract = Nothing;
		}
		return false;
	}

    protected bool Nothing() { return false; }
}

