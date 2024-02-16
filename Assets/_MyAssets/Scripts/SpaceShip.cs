using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
	public static SpaceShip Instance;

	private bool _isFrozen = false;
	private Vector3 current_speed = Vector3.zero;
	private int current_pu = 0;

	[Header("Stats")]
	[SerializeField] private float max_speed = default;
	[SerializeField] private float acceleration = default;
	[SerializeField] private float airbrake_power = default;
	[SerializeField] private int hp = default;
	[SerializeField] private float weigth = default;
	[SerializeField] private float agility = default;

	private Rigidbody _rb;

	private Vector3 _dragForce;
	private const float COEF_DRAG = -0.2f;

	/********************************************
					SYSTÈME PID
	*********************************************/
	/****** CONSTANTES ******/
	private float GRAVITY = 9.81f;							// constante gravité
	private const float MAX_DIST = 0.5f;						// distance maximale du raycast
	private const float HAUTEUR_TARGET = MAX_DIST * 0.7f;	// hauteur que le PID vise (70% la distance maximale du raycast)

	/* (P) PROPORTIONAL
		proportionnel à _diffHauteur,
		l'augmenter fait que le systçème PID agit plus agressivement en fonction de _diffHauteur,
		augmenter cette constante peut causer de l'overshoot et donc de l'oscillation */
	private const float PID_KP = 200f;

	/* (I) INTEGRAL - [ IMPLEMENTER PLUS TARD ]
		l'intégrale (accumule _diffHauteur) aide à contrer les forces constantes,
		une valeur trop haute ici peut causer instabilité
		et ralentir l'effet du système PID */
	//private const float PID_KI = 0.1f;

	/* (D) DERIVATIVE
		la dérivée (_deriveeHauteur) prédit _diffHauteur du future,
		évite d'overshoot donc réduit l'oscillation,
		PID_KD permet de controller l'intensité de
		cet effet "anti-overshoot" */
	private const float PID_KD = 500f;

	/****** VARIABLES ******/
	[SerializeField] private LayerMask _layersToHit;		// dans l'inspecteur mettre la layer "track". TODO: trouver comment faire sans l'inspecteur, la layer sera toujours "track"
	private Vector3 _rayDir = Vector3.down;					// direction de la gravité, par défaut vers le bas, toujours normalisé
	private float _hauteur = 0f;							// hauteur actuelle
	private float _diffHauteur = 0f;						// difference (delta distance) entre la hauteur actuelle et la hauteur que le PID vise
	private float _lastDiffHauteur = 0f;					// même chose que _diffHauteur mais de la dernière frame (du FixedUpdate())
	private float _deriveeHauteur = 0f;						// derivee de la hauteur (vitesse du deplacement vertical)
	private float _PIDForce = 0f;							// force verticale à appliquer pour faire léviter le spaceship (PID)
	private bool _onGround = false;							// si le ray touche le sol (et donc si le PID est active)


	// Awake(), Start(), Update() et FixedUpdate() sont tous private implicitement, pas besoin de mettre private devant

	void Awake()
	{
		Instance = this; //associe l'instance de Player au script
		_rb = GetComponent<Rigidbody>();
	}

	void Start()
	{

	}

	void Update()
	{
		current_speed = _rb.velocity;
		Debug.Log(_rb.velocity.magnitude);
	}

	// called à chaque Time.fixedDeltaTime (0.02s par défaut)
	void FixedUpdate()
	{
		AirResistance();
		CheckForGround();
		PID();
		ApplyVerticalForce();
	}

	// méthode privée qui trouve la direction de la gravité
	private void CheckForGround()
	{
		Ray ray = new Ray(transform.position, _rayDir);
		if(Physics.Raycast(ray, out RaycastHit hit, MAX_DIST, _layersToHit, QueryTriggerInteraction.Ignore)) {
			_rayDir = -hit.normal;
			_onGround = true;
			_hauteur = hit.distance;
			//Debug.Log("found ground");
			Debug.DrawLine(transform.position, hit.point, Color.red, Time.fixedDeltaTime);							// direction de la gravité
			Debug.DrawLine(transform.position, transform.position + (hit.normal), Color.blue, Time.fixedDeltaTime);	// normale de la surface (inverse de la direction de la gravité)
		} else {
			//Debug.Log("ground not found");
			Debug.DrawLine(transform.position, transform.position + _rayDir, Color.red, Time.fixedDeltaTime);		// direction de la gravité (sans utiliser hit.point)
			_onGround = false;
		}
	}

	// trouve la force verticale qu'il faut appliquer pour faire léviter le spaceship
	// https://en.wikipedia.org/wiki/Proportional%E2%80%93integral%E2%80%93derivative_controller
	private void PID()
	{
		_diffHauteur = HAUTEUR_TARGET - _hauteur;							// trouve delta distance
		_deriveeHauteur = _diffHauteur - _lastDiffHauteur;					// trouve la derivee
		_PIDForce = _diffHauteur * PID_KP + _deriveeHauteur * PID_KD;		// force PID qu'il faudrait appliquer
		_lastDiffHauteur = _diffHauteur;									// enregistre le delta distance de la frame actuelle pour l'utiliser comme _lastDiffHauteur dans la prochaine frame
	}

	// "vertical" est relatif au spaceship ici, c'est l'axe Z local, non global
	private void ApplyVerticalForce()
	{
		if(_onGround) {
			_rb.AddForce(_rayDir * -_PIDForce, ForceMode.Acceleration);	// ForceMode.Acceleration ignore la masse et applique directement l'accèleration
		} else {
			_rb.AddForce(_rayDir * GRAVITY, ForceMode.Acceleration);
		}
	}

	private void AirResistance()
	{
		//Vector3 force = new Vector3(-max_speed * (current_speed.x / max_speed), -max_speed * (current_speed.y / max_speed), -max_speed * (current_speed.z / max_speed))/2;
		
		_dragForce = _rb.velocity * COEF_DRAG;
		_rb.AddForce(_dragForce, ForceMode.Acceleration);
	}

	public void Forward()
	{
		if (!_isFrozen && current_speed.y < max_speed)
		{
			Vector3 force = new Vector3(-1 * acceleration, 0, 0);
			_rb.AddForce(force);
		}
	}

	public void Turn(bool left)
	{
		if (!_isFrozen)
		{
			if (left)
			{
				float rotation = agility - current_speed.y;
			}
			if (!left)
			{
				float rotation = (agility - current_speed.y) * -1;
			}
		}
	}

	public void AirBrake(bool left)
	{
		if (!_isFrozen)
		{
			if (left)
			{
				float rotation = agility - current_speed.y + airbrake_power;
				Vector3 force = new Vector3(1 * current_speed.y + airbrake_power - weigth, 0, 0);
				_rb.AddForce(force);
			}
			if (!left)
			{
				float rotation = (agility - current_speed.y + airbrake_power) * -1;
				Vector3 force = new Vector3(1 * current_speed.y + airbrake_power - weigth, 0, 0);
				_rb.AddForce(force);
			}
		}
	}

	public void UsePU()
	{
		if (!_isFrozen)
		{

		}
	}

	void RemovePU()
	{

	}

	void AddPU()
	{

	}
}