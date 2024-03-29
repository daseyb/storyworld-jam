﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RitualGameManager : MonoBehaviour {

    public int PlacedCandlesCount { get { return m_CandleManager.GetCandleList().Count; } }

    private SymbolManager m_SymbolManager = null;
    private CandleManager m_CandleManager = null;

    private RitualInfo m_CurrentRitualInfo;
    [HideInInspector]
    public RitualInfo m_TargetRitualInfo;
    [HideInInspector]
    public GameObject m_PersistantRitual;

    private const int MAX_CANDLE_DISTANCE_FROM_SYMBOL = 275;
    private const float CANDLE_PERSPECTIVE_CORRECTION_FACTOR = 0.4f;


    public bool b_Debug = false;

	// Use this for initialization
	void Start () {

        this.m_PersistantRitual = new GameObject();
        this.m_PersistantRitual.name = "PersistantRitual";

        this.m_CurrentRitualInfo = ScriptableObject.CreateInstance<RitualInfo>();

        this.m_SymbolManager = GameObject.FindObjectOfType<SymbolManager>();
        if (this.m_SymbolManager == null)
        {
            Debug.LogWarning("No symbolManager found!.");
            Destroy(this);
        }

        this.m_CandleManager = GameObject.FindObjectOfType<CandleManager>();
        if (this.m_CandleManager == null)
        {
            Debug.LogWarning("Null candlemanager.");
            Destroy(this);
        }

        this.m_TargetRitualInfo = ScriptableObject.CreateInstance<RitualInfo>();

        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("Null gameManager.");
            Destroy(this);
            return;
        }

        Debug.Log("Illness to cure: " + gameManager.ActiveModel.CurrentIllness.Name);

        //symbol type - based on affected body part
        this.m_TargetRitualInfo.SymbolType = (SymbolTypes.SymbolType) gameManager.ActiveModel.IllBodyPart;
        this.m_TargetRitualInfo.SymbolPosition = new Vector2(Screen.width/2.0f, Screen.height *0.4f);

        this.m_TargetRitualInfo.CandlePositions = new List<Vector2>();

        //candle configuration - based on illness
        CandleConfigurations.CandleConfig canConfig = gameManager.ActiveModel.CurrentIllness.CandleConfig;

        this.m_TargetRitualInfo.CandlePositions = CandleconfigurationHelper.GetCandlePositions(canConfig);

        //convert relative normalised candle configuration positions to screen space
        for (int i = 0; i < this.m_TargetRitualInfo.CandlePositions.Count; i++)
        {
            Vector2 candlePos = this.m_TargetRitualInfo.CandlePositions[i];
            this.m_TargetRitualInfo.CandlePositions[i] = this.m_TargetRitualInfo.SymbolPosition + new Vector2(candlePos.x * MAX_CANDLE_DISTANCE_FROM_SYMBOL, candlePos.y * MAX_CANDLE_DISTANCE_FROM_SYMBOL*((candlePos.y < 0.0f)?CANDLE_PERSPECTIVE_CORRECTION_FACTOR:1.0f));
        }

        this.DebugPrintTargetRitualInfo();

        if (b_Debug)
        {

            this.m_SymbolManager.EnableDebug();

            this.m_CandleManager.EnableDebug();
        }

	}

    public float GetHeuristicValue()
    {
        //get candle pos
        this.m_CurrentRitualInfo.CandlePositions = this.m_CandleManager.GetCandlePositionsList();
        var val = this.m_TargetRitualInfo.GetHeuristicValue(this.m_CurrentRitualInfo);
        Debug.Log("Ritual heuristic value: " + val);
        return val;
    }

    public void SetTargetRitual(RitualInfo target)
    {
        this.m_TargetRitualInfo = target;
    }

    public void SetSymbolInfo(SymbolTypes.SymbolType type, Vector2 pos)
    {
        this.m_CurrentRitualInfo.SymbolType = type;
        this.m_CurrentRitualInfo.SymbolPosition = pos;
    }

    private void DebugPrintTargetRitualInfo()
    {
        Debug.Log("TargetSymbolType: " + this.m_TargetRitualInfo.SymbolType);
        //Debug.Log("TargetSymbolPos: " + this.m_TargetRitualInfo.SymbolPosition);
        Debug.Log("TargetCandleNum: " + this.m_TargetRitualInfo.CandlePositions.Count);

        /*
        for (int i = 0; i < this.m_TargetRitualInfo.CandlePositions.Count; i++)
        {
            Debug.Log("CandlePos" + i + ": " + this.m_TargetRitualInfo.CandlePositions[i]);
        }
         * */
    }
}
