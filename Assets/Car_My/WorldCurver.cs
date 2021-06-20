using UnityEngine;

[ExecuteInEditMode]
public class WorldCurver : MonoBehaviour
{
	[Range(-0.1f, 0.1f)]
	[SerializeField] private float maxCurveStrength = 0.01f;

	private float targetCurveStrength;
	private float curveStrength;

	public bool changeViaTime;
	[SerializeField] private float changeTime;

	private float currentTimer;

	int m_CurveStrengthID;

    private void OnEnable()
    {
	    currentTimer = 0;
	    curveStrength = maxCurveStrength;
	    targetCurveStrength = -maxCurveStrength;
        m_CurveStrengthID = Shader.PropertyToID("_CurveStrength");
    }

	void Update()
	{
		Shader.SetGlobalFloat(m_CurveStrengthID, curveStrength);
		
		if (!changeViaTime) return;

		currentTimer += Time.deltaTime;
		curveStrength = Mathf.Lerp(-targetCurveStrength, targetCurveStrength, currentTimer / changeTime);
		if (currentTimer >= changeTime)
		{
			currentTimer = 0;
			targetCurveStrength *= -1;
		}
	}
}
