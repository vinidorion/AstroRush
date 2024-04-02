using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private float max_speed = default;
	[SerializeField] private float _accel = default;
	[SerializeField] private float airbrake_power = default;
	private const int MAX_HP = 100;
	private int _hp = MAX_HP;
	[SerializeField] private float weight = default;
	[SerializeField] private float agility = default;
	[SerializeField] private float _slower = default;
	[SerializeField] private float _maxBoost = default;
	[SerializeField] private float _boost = default;

	private bool _isFrozen = false;
	private Vector3 current_speed = Vector3.zero;
	private Rigidbody _rb;
	private GameManager _gm;

	[SerializeField] private int _pu = -1; //[SerializeField] temporaire pour tester

	// WAYPOINTS / POSITIONS
	private int _lap = 0;
	private int _waypoint = 0;
	private int _position;

	// le Time.time quand le lap est complété
	// premier lap :		_listLapTime[0] - _startTime
	// autre que premier : 	_listLapTime[n] - _listLapTime[n - 1]
	// temps total :		_listLapTime[_listLapTime.Count - 1] - _startTime
	private List<float> _listLapTime = new List<float>(); 

	// PHYSICS
	private Vector3 _dragForce;
	private const float COEF_DRAG = -0.3f;
	private Vector3 _prevRayDir = Vector3.down;	


	/********************************************
					SYSTÈME PID
	*********************************************/

	/****** CONSTANTES ******/
	private const float GRAVITY = 9.81f;					// constante gravité
	private const float MAX_DIST = 0.5f;					// distance maximale du raycast
	private const float HAUTEUR_TARGET = MAX_DIST * 0.7f;	// hauteur que le PID vise (70% la distance maximale du raycast)

	/* (P) PROPORTIONAL
		proportionnel à _diffHauteur,
		l'augmenter fait que le système PID agit plus agressivement en fonction de _diffHauteur,
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
	private LayerMask _layersToHit;							// la seule layer que le Raycast du PID touche
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
		_rb = GetComponent<Rigidbody>();
		_rb.angularDrag = 10f;
		_layersToHit = 1 << LayerMask.NameToLayer("track");
	}

	void Start()
	{
		_gm = GameManager.Instance;
	}

	void Update()
	{
		current_speed = _rb.velocity;
		//Debug.Log(_rb.velocity.magnitude);
	}

	// called à chaque Time.fixedDeltaTime (0.02s par défaut)
	void FixedUpdate()
	{
		if (!_isFrozen) {
			// physics
			AirResistance();
			LateralStability();

			// PID/gravité
			CheckForGround();
			ApplyVerticalForce();
		}
	}

	/********************************************
					PHYSICS
	*********************************************/

	// méthode privée qui trouve la direction de la gravité
	private void CheckForGround()
	{
		Ray ray = new Ray(transform.position, _rayDir);
		if(Physics.Raycast(ray, out RaycastHit hit, MAX_DIST, _layersToHit, QueryTriggerInteraction.Ignore)) {
			_prevRayDir = -_rayDir;
			_rayDir = -hit.normal;
			_onGround = true;
			_hauteur = hit.distance;
			PID();
			KeepUpright();
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
			_rb.AddForce(_rayDir * -_PIDForce, ForceMode.Acceleration);	// ForceMode.Acceleration ignore la masse et applique directement l'accèleration
		} else {
			_rb.AddForce(_rayDir * GRAVITY, ForceMode.Acceleration);
		}
	}

	private void AirResistance()
	{
		//Vector3 force = new Vector3(-max_speed * (current_speed.x / max_speed), -max_speed * (current_speed.y / max_speed), -max_speed * (current_speed.z / max_speed))/2;
		
		_dragForce = _rb.velocity * COEF_DRAG;
		_rb.AddForce((_dragForce * _slower), ForceMode.Acceleration);
	}

	// force appliquée latéralement pour éviter que le spaceship glisse sur le côté
	private void LateralStability()
	{
		float lateralSpeed = transform.InverseTransformDirection(_rb.velocity).x;
		//Debug.Log("lateral speed: " + lateralSpeed.ToString("F2"));

		if(lateralSpeed > 1f) {
			_rb.AddForce(transform.right * -5f);
		} else if(lateralSpeed < -1f)  {
			_rb.AddForce(transform.right * 5f);
		}
	}

	private void KeepUpright()
	{
		Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -_rayDir) * transform.rotation;
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
	}

	/********************************************
				POSITIONS/WAYPOINTS
	*********************************************/

	// utilisé dans la classe LapComplete
	public void LapCompleted()
	{
		_lap++; // et changer lap dans le hud
		GetComponent<WaypointFinder>().SetWaypoint(0);

		// check ici si _lap atteint le nombre de lap total, si oui et si c'est le joueur: c'est la fin du jeu (Camera.Instance.SetCameraMode(CameraMode.Spectate);)

		_listLapTime.Add(Time.time);
		if (_listLapTime.Count == 1) {
			InGameHud.Instance.TimeComp(_listLapTime[0] - GameManager.Instance.GetStartTime());
		} else {
			InGameHud.Instance.TimeComp(_listLapTime[_listLapTime.Count - 1] - _listLapTime[_listLapTime.Count - 2]);
		}
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

	// méthode public pour manuellement set le waypoint
	public void SetWaypoint(int point)
	{
		_waypoint = point;
	}

	// utilisé dans PosManager
	public void SetPosition(int pos)
	{
		_position = pos;
	}

	// utilisé dans PosManager
	public int GetPosition()
	{
		return _position;
	}

	// cette valeur représente la "position" du spaceship
	// plus haute = premier
	// plus basse = dernier
	// faut juste sort en ordre décroissant pour trouver l'ordre des spaceships (le sort se fait dans la classe PosManager)
	// la multiplication par 1000 ici implique qu'on ne peut avoir que 1000 waypoints max
	public int GetPosValue()
	{
		return (_lap * 1000) + _waypoint;
	}

	/********************************************
					CONTROLS
	*********************************************/

	public void Forward()
	{
		_rb.AddForce(transform.forward * _accel /* * (_slower + 1)*/, ForceMode.Acceleration);
	}

	public void backward()
	{
		_rb.AddForce(-1 * transform.forward * _accel /* * (_slower + 1)*/, ForceMode.Acceleration);
	}

	public void Turn(bool left)
	{
		if (left) {
			//Debug.Log("TURNING LEFT: " + _rb.angularVelocity.magnitude);
			//float rotation = agility - current_speed.y;
			_rb.AddTorque(transform.up * /*_rb.angularDrag **/ -agility, ForceMode.Acceleration);
			//_rb.angularVelocity = new Vector3(0f, 6f, 0f);
		} else {
			//Debug.Log("TURNING RIGHT: " + _rb.angularVelocity.magnitude);
			_rb.AddTorque(transform.up * /*_rb.angularDrag **/ agility, ForceMode.Acceleration);
			//float rotation = (agility - current_speed.y) * -1;
		}
	}

	public void AirBrake(bool left)
	{
		if (left) {
			float rotation = agility - current_speed.y + airbrake_power;
		} else {
			float rotation = (agility - current_speed.y + airbrake_power) * -1;
		}
		Vector3 force = -1 * _rb.velocity * airbrake_power / weight;
		_rb.AddForce(force * 15f);
	}

	/********************************************
					POWER UPS
	*********************************************/

	public void UsePU()
	{
		if (!_isFrozen && _pu != -1) {
			poly.PU pu = Instantiate(_gm.GetGameObjectPU(_pu), transform.position + (transform.forward * 0.5f), Quaternion.LookRotation(transform.forward)).GetComponent<poly.PU>();
			pu.SetOwner(transform);
			//_pu = -1;  //en commentaire pour tester
		}
	}

	// méthode publique qui donne un item au spaceship
	// sélectionne l'item en fonction de la position
	// y = a * ( x - 0.5 ) + 0.5
	public void GivePU()
	{
		if(_pu != -1) {
			return;
		}
		
		int numShip = FindObjectsOfType<SpaceShip>().Length;
		int pos = _position;
		int numPU = _gm.GetNumPUs();
		int[] listWeight = new int[numPU];
		float ratioPos = ((1f - (pos / (float)numShip)) * 2f) - 1f;

		for(int i = 0; i < numPU; i++) {
			listWeight[i] = Mathf.RoundToInt((ratioPos * ((i / (float)(numPU - 1)) - 0.5f) + 0.5f) * numPU * 100f);
		}

		/*Debug.Log("position: " + pos);
		for(int i = 0; i < listWeight.Length; i++) {
			Debug.Log(i + " : " + listWeight[i]);
		}*/

		int totalWeight = 0;

		for (int i = 0; i < numPU; i++) {
			totalWeight += listWeight[i];
		}

		int random = Random.Range(0, totalWeight);

		for (int i = 0; i < numPU; i++) {
			if (random < listWeight[i]) {
				_pu = i;
				Debug.Log("PU PICKED: " + _gm.GetGameObjectPU(_pu).name.Substring(3));
				break;
			}
			random -= listWeight[i];
		}
	}

	public int GetMaxHP()
	{
		return MAX_HP;
	}

	public int GetHP()
	{
		return _hp;
	}

	public void GiveHP(int life)
	{
		_hp = Mathf.Clamp(_hp + life, 0, MAX_HP);

		//Debug.Log(life + " (" + _hp + " HP)");
	}

	public float GetBoost()
	{
		return _boost;
	}

	public void SetBoost(float boost)
	{
		_boost = boost;
		if (_boost > _maxBoost) {
			_boost = _maxBoost;
		}
	} 

	// méthode publique qui permet de freeze/unfreeze le spaceship
	public void Freeze(bool isFrozen)
	{
		_isFrozen = isFrozen;
	}

	// méthode publique pour savoir si le spaceship est frozen
	public bool isFrozen()
	{
		return _isFrozen;
	}

	// temporaire
	public Vector3 GetVecGrav()
	{
		return _rayDir;
	}

	// méthode publique qui permet de ralentir le spaceship (par les PU)
	public void Slow(float slow, float slowTime)
	{
		_slower = slow;
		StartCoroutine(SlowTime(slowTime));
	}

	IEnumerator SlowTime(float slowTime) 
	{ 
		yield return new WaitForSeconds(slowTime);
		_slower = 0;
	}
}