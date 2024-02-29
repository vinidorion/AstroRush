using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
	public static SpaceShip Instance;

	[Header("Stats")]
	[SerializeField] private float max_speed = default;
	[SerializeField] private float _accel = default;
	[SerializeField] private float airbrake_power = default;
	[SerializeField] private int max_hp = default;
	[SerializeField] private float weigth = default;
	[SerializeField] private float agility = default;

	private bool _isFrozen = false;
	private Vector3 current_speed = Vector3.zero;
	private int _currentPU = 0;
	private Rigidbody _rb;

	// pour trouver position
	private int _lap = 0;
	private int _waypoint = 0;

	// le Time.time quand le lap est complété
	// faire _listLapTime[0] - _startTime pour le temps du premier lap
	// faire _listLapTime[n] - _listLapTime[n - 1] pour le temps de lap autre que le premier
	// faire _listLapTime[_listLapTime.Count - 1] - _startTime pour le temps total (quand le dernier lap est complété)
	private List<float> _listLapTime = new List<float>(); 

	private Vector3 _dragForce;
	private const float COEF_DRAG = -0.2f;

	public int GetLife() { return max_hp; }

	/********************************************
					SYSTÈME PID
	*********************************************/

	/****** CONSTANTES ******/
	private float GRAVITY = 9.81f;							// constante gravité
	private const float MAX_DIST = 0.5f;					// distance maximale du raycast
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

	// comment ignorer angular impulse des collisions en gardant la capacité d'appliquer .AddTorq()
	// https://forum.unity.com/threads/how-to-stop-the-rotation-of-a-rigidbody-but-for-specific-collisions.1189615/#:~:text=I%20finally%20find,you%27re%20awesome%20guys


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
		_rb.velocity = transform.forward * _rb.velocity.magnitude;
		
		//Debug.Log(_rb.velocity.magnitude);
	}

	// called à chaque Time.fixedDeltaTime (0.02s par défaut)
	void FixedUpdate()
	{
		if (!_isFrozen) {
			AirResistance();
			CheckForGround();
			ApplyVerticalForce();
			Waypoints();
		}
	}

	// méthode privée qui trouve la direction de la gravité
	private void CheckForGround()
	{
        Ray ray = new Ray(transform.position, _rayDir);
		if(Physics.Raycast(ray, out RaycastHit hit, MAX_DIST, _layersToHit, QueryTriggerInteraction.Ignore)) {
			_rayDir = -hit.normal;
			_onGround = true;
			_hauteur = hit.distance;
			PID();
			//Debug.Log("found ground");
			Debug.DrawLine(transform.position, hit.point, Color.red, Time.fixedDeltaTime);							// direction de la gravité
			Debug.DrawLine(transform.position, transform.position + (hit.normal), Color.blue, Time.fixedDeltaTime); // normale de la surface (inverse de la direction de la gravité)
			transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
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
		// pas de division par deltaTime, c'est toujours 0.02s (FixedUpdate())
		// donc on fait le calcul directement sur KP, KD et KI
		// pour éviter de faire le calcul à chaque frame
		// c'est pourquoi ces constantes ont des valeurs si hautes
		_PIDForce = _diffHauteur * PID_KP + _deriveeHauteur * PID_KD;		// force PID qu'il faudrait appliquer
		_lastDiffHauteur = _diffHauteur;									// enregistre le delta distance de la frame actuelle pour l'utiliser comme _lastDiffHauteur dans la prochaine frame
	}

	// "vertical" est relatif au spaceship ici, c'est par rapport à _rayDir (direction de la gravité)
	private void ApplyVerticalForce()
	{
		if(_onGround) {
            _rb.AddForce(_rayDir * -_PIDForce, ForceMode.Acceleration); // ForceMode.Acceleration ignore la masse et applique directement l'accèleration
			//Debug.Log(_rayDir);
			//transform.up = -_rayDir;
        } else {
			_rb.AddForce(_rayDir * GRAVITY, ForceMode.Acceleration);
		}

		
	}

	// trouve le waypoint du spaceship
	private void Waypoints()
	{
		Vector3 waypointPos = WaypointManager.Instance.GetWaypointPos(_waypoint);			// position du current waypoint
		Vector3 nextwaypointPos = WaypointManager.Instance.GetWaypointPos(_waypoint + 1);	// position du next waypoint

		//Debug.Log("waypointPos: " + waypointPos);
		//Debug.Log("nextwaypointPos: " + nextwaypointPos);

		// pour visualiser à quel waypoint le spaceship est rendu
		Debug.DrawLine(transform.position, waypointPos, Color.green, Time.fixedDeltaTime);
		Debug.DrawLine(transform.position, nextwaypointPos, Color.blue, Time.fixedDeltaTime);

		// sqrt inutile ici, on compare deux distances
		float distCurrWaypoint = (transform.position - waypointPos).sqrMagnitude;
		float distNextWaypoint = (transform.position - nextwaypointPos).sqrMagnitude;

		if(distNextWaypoint < distCurrWaypoint) {
			_waypoint++;
			if(WaypointManager.Instance.IsFinalWaypoint(_waypoint)) {
				_waypoint = 0;
				_lap++; // et changer lap dans le hud

				// check ici si _lap atteint le nombre de lap total, si oui c'est la fin du jeu

				_listLapTime.Add(Time.time);
				if(_listLapTime.Count == 1) {
					InGameHud.Instance.TimeComp(_listLapTime[0] - GameManager.Instance.GetStartTime());
				} else {
					InGameHud.Instance.TimeComp(_listLapTime[_listLapTime.Count - 1] - _listLapTime[_listLapTime.Count - 2]);
				}
			}
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
		if (!_isFrozen/* && current_speed.x < max_speed*/)
		{
			//Vector3 force = new Vector3(-1 * acceleration, 0, 0);
			_rb.AddForce(transform.forward * _accel);
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

	// méthode publique qui donne un item au spaceship
	public void GivePU()
	{
		Debug.Log("RECEIVED PU");
		// random ici pour choisir le PU
		// plus on est dernier, meilleur sont nos PU, vice versa
	}

	// méthode publique qui retourne le nombre de lap du spaceship
	public int GetLap()
	{
		return _lap;
	}

	// méthode publique qui retourne l'index du waypoint actuel du spaceship
	public int GetWaypoint()
	{
		return _waypoint;
	}

	// temporaire
	public Vector3 GetVecGrav(){
		return _rayDir;
	}

	// méthode publique qui permet de freeze/unfreeze le spaceship
	public void Freeze(bool isFrozen)
	{
		_isFrozen = isFrozen;
	}
}