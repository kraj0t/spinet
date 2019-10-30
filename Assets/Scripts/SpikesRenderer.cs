using UnityEngine;
using System.Collections;

public class SpikesRenderer : MonoBehaviour {

	public SpikesController spikesController;

	public Renderer[] spikeVisuals;
		

	void Update () {
		foreach ( Renderer r in spikeVisuals ) {
			r.enabled = spikesController._pinchosFuera;
		}
	}
}
