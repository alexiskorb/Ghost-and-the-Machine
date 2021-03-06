﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHealth : MonoBehaviour
{
	[Tooltip("The time it takes for one damager to kill the ghost.")]
	[SerializeField]
	private float m_timeToKill = 2f;

	[Tooltip("The time it takes for the ghost to regenerate to full.")]
	[SerializeField]
	private float m_timeToRegen = 0.5f;

	[SerializeField]
	private GameObject[] m_onDeathSpawnPrefabs = null;

	[SerializeField]
	private AudioClip m_gettingDamagedAudio = null;

	/// <summary>
	/// The ghost's current health.
	/// </summary>
	private float m_health = 1f;

	/// <summary>
	/// True if the ghost is currently being damaged.
	/// </summary>
	public bool IsBeingDamaged { get; private set; }

	#region Cached Components

	private AudioSource m_audioSource;
	private CircleCollider2D m_collider;

	#endregion

	private void Awake()
	{
		m_audioSource = OurUtility.GetOrAddComponent<AudioSource>(gameObject);
		m_collider = GetComponent<CircleCollider2D>();
	}

	void Update()
	{
		float hitCount = GhostDamager.DamageAmount(transform.position, m_collider.radius);
		if (hitCount > 0f)
		{
			if (!IsBeingDamaged && m_gettingDamagedAudio)
			{
				m_audioSource.PlayOneShot(m_gettingDamagedAudio, 0.5f);
			}
			IsBeingDamaged = true;

			m_health -= Time.deltaTime * hitCount / m_timeToKill;
			if (m_health <= 0f)
			{
				Destroy(gameObject);
				foreach (GameObject spawnPrefab in m_onDeathSpawnPrefabs)
				{
					Instantiate(spawnPrefab, transform.position, Quaternion.identity);
				}
			}
		}
		else
		{
			IsBeingDamaged = false;

			m_health = Mathf.Clamp01(m_health + Time.deltaTime / m_timeToRegen);
		}
	}
}
