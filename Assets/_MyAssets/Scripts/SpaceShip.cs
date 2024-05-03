using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpaceShip : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private float max_speed = default;
	[SerializeField] private float _accel = default;
	private const float FORWARD_ACCEL = 0.7f;
	private const float BACKWARD_ACCEL = -0.5f;
	[SerializeField] private float _airbrakePower = default;
	private const int MAX_HP = 100;
	private int _hp = MAX_HP;
	[SerializeField] private float _weight = default;
	[SerializeField] private float _agility = default;
	[SerializeField] private float _slower = default;
	[SerializeField] private float _maxBoost = default;
	[SerializeField] private float _size = default;
	[SerializeField] private float _boost = default;

	[SerializeField] private int _pu = -1; //[SerializeField] temporaire pour tester
	[SerializeField] private GameObject fire = default;

	// keep upright
	private Vector3 _rayDirFront = Vector3.down;
	private Vector3 _rayDirBackRight = Vector3.down;
	private Vector3 _rayDirBackLeft = Vector3.down;
	private bool _onGround1 = false;
	private bool _onGround2 = false;
	private bool _onGround3 = false;
	// objets d'ou partent les raycast pour keep upright
	[SerializeField] private GameObject _front;
	[SerializeField] private GameObject _backleft;
	[SerializeField] private GameObject _backright;
	private Vector3 _fronthit;
	private Vector3 _backrighthit;
	private Vector3 _backlefthit;

	// WAYPOINTS / POSITIONS
	private int _lap = 0;
	private WaypointFinder _waypoint;
	private int _position;

	// le Time.time quand le lap est complété
	// premier lap :		_listLapTime[0] - _startTime
	// autre que premier :	_listLapTime[n] - _listLapTime[n - 1]
	// temps total :		_listLapTime[_listLapTime.Count - 1] - _startTime
	private List<float> _listLapTime = new List<float>();

	private const float COEF_DRAG = -0.2f;
	private Vector3 _forwardSpeed = Vector3.zero;

	private bool _isPly = false;
	private bool _isFrozen = false;
	private Rigidbody _rb;
	private GameManager _gm;

	/********************************************
					SYSTÈME PID
	*********************************************/

	/****** CONSTANTES ******/
	private const float GRAVITY = 9.81f;					// constante gravité
	private const float MAX_DIST = 0.5f;					// distance maximale du raycast
	private const float HAUTEUR_TARGET = MAX_DIST * 0.7f;	// hauteur que le PID vise (70% la distance maximale du raycast)

	/* (P) PROPORTIONAL
		multiplie l'erreur,
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
	private LayerMask _layersToHit;				// la seule layer que le Raycast du PID touche
	private Vector3 _rayDir = Vector3.down;		// direction de la gravité, par défaut vers le bas, toujours normalisé
	private float _hauteur = 0f;				// hauteur actuelle
	private float _lastDiffHauteur = 0f;		// même chose que _diffHauteur mais de la dernière frame (du FixedUpdate())
	private float _PIDForce = 0f;				// force verticale à appliquer pour faire léviter le spaceship (PID)


	void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_waypoint = GetComponent<WaypointFinder>();
		_rb.angularDrag = 10f;
		_layersToHit = 1 << LayerMask.NameToLayer("track");

		if (max_speed == 0f) {
			max_speed = 10f;
		}
	}

	void Start()
	{
		_gm = GameManager.Instance;
		_isPly = this.gameObject == Player.Instance.gameObject;
	}

	void Update()
	{
		_forwardSpeed = Vector3.Project(_rb.velocity, transform.forward);
		if(fire) {
			if(!Input.GetKey(KeyCode.W) && fire.activeSelf) fire.SetActive(false);
		}
	}

	// called à chaque Time.fixedDeltaTime (0.02s par défaut)
	void FixedUpdate()
	{
		if (!_isFrozen) {
			// physics
			AirResistance();
			LateralStability();

			// PID/gravité
			Levitate();
		}
	}

	/********************************************
					PHYSICS
	*********************************************/

	// les variables sont passées par référence
	private void ThrowRay(ref bool onGround, ref Vector3 rayOrigin, ref Vector3 rayDir, ref GameObject ob)
	{
		Ray ray = new Ray(ob.transform.position, rayDir);
		if (Physics.Raycast(ray, out RaycastHit hit, MAX_DIST, _layersToHit, QueryTriggerInteraction.Ignore)) {
			rayOrigin = hit.point;
			rayDir = -hit.normal;
			onGround = true;
			//Debug.DrawLine(transform.position, hit.point, Color.red, Time.fixedDeltaTime);
		} else {
			onGround = false;
		}
	}

	// méthode privée qui fait léviter le spaceship
	private void Levitate()
	{
		/*ThrowRay(ref _onGround1, ref _fronthit, ref _rayDirFront, ref _front);
		ThrowRay(ref _onGround2, ref _backlefthit, ref _rayDirBackLeft, ref _backleft);
		ThrowRay(ref _onGround3, ref _backrighthit, ref _rayDirBackRight, ref _backright);*/

		_onGround3 = _onGround1 && _onGround2 && _onGround3;

		Vector3 directionGrav = _rayDir;

		Ray ray = new Ray(transform.position, _rayDir);
		if (Physics.Raycast(ray, out RaycastHit hit, MAX_DIST, _layersToHit, QueryTriggerInteraction.Ignore)) {
			_rayDir = -hit.normal;
			_hauteur = hit.distance;
			PID();
			directionGrav *= -_PIDForce;
			KeepUpright(false);
			Debug.DrawLine(transform.position, hit.point, Color.red, Time.fixedDeltaTime);							// direction de la gravité
			Debug.DrawLine(transform.position, transform.position + (hit.normal), Color.blue, Time.fixedDeltaTime);	// normale de la surface (inverse de la direction de la gravité)
		} else {
			directionGrav *= GRAVITY;
			Debug.DrawLine(transform.position, transform.position + _rayDir, Color.red, Time.fixedDeltaTime);		// direction de la gravité (sans utiliser hit.point)
		}

		_rb.AddForce(directionGrav, ForceMode.Acceleration); // ForceMode.Acceleration ignore la masse et applique directement l'accèleration
	}

	// trouve la force verticale qu'il faut appliquer pour faire léviter le spaceship
	// https://en.wikipedia.org/wiki/Proportional%E2%80%93integral%E2%80%93derivative_controller
	private void PID()
	{
		float _diffHauteur = HAUTEUR_TARGET - _hauteur;						// difference (delta distance) entre la hauteur actuelle et la hauteur que le PID vise
		float _deriveeHauteur = _diffHauteur - _lastDiffHauteur;			// derivee (taux de variation de l'erreur)
		// pas de division par deltaTime, c'est toujours 0.02s (FixedUpdate())
		// donc on fait le calcul directement sur KP, KD et KI
		// pour éviter de faire le calcul à chaque frame
		// c'est pourquoi ces constantes ont des valeurs si hautes
		_PIDForce = _diffHauteur * PID_KP + _deriveeHauteur * PID_KD;		// force PID qu'il faudrait appliquer
		_lastDiffHauteur = _diffHauteur;									// enregistre le delta distance de la frame actuelle pour l'utiliser comme _lastDiffHauteur dans la prochaine frame
	}

	private void AirResistance()
	{
		Vector3 dragForce = _rb.velocity * (-GetForwardSpeed() / max_speed);

		// division par 0 donne des erreurs
		if (!IsVecValid(dragForce)) {
			return;
		}

		_rb.AddForce(dragForce/*_rb.velocity * COEF_DRAG*/, ForceMode.Acceleration);
	}

	// force appliquée latéralement pour éviter que le spaceship glisse sur le côté
	private void LateralStability()
	{
		float lateralSpeed = transform.InverseTransformDirection(_rb.velocity).x;
		//Debug.Log($"lateral speed: {lateralSpeed.ToString("F2")}");
		
		if(lateralSpeed > 1f) {
			_rb.AddForce(transform.right * -5f * _agility, ForceMode.Acceleration);
		} else if(lateralSpeed < -1f) {
			_rb.AddForce(transform.right * 5f * _agility, ForceMode.Acceleration);
		}
	}

	private void KeepUpright(bool ground)
	{
		Vector3 direction = -_rayDir;

		if (ground) {
			Vector3 frontToLeft = _fronthit - _backlefthit;
			Vector3 frontToRight = _fronthit - _backrighthit;
			direction = Vector3.Cross(frontToRight, frontToLeft);
		}

		Quaternion targetRotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4f * Time.fixedDeltaTime);
	}

	/********************************************
				POSITIONS/WAYPOINTS
	*********************************************/

	// utilisé dans la classe LapComplete
	public void LapCompleted()
	{
		Debug.Log("LapCompleted() called");
		_waypoint.SetWaypoint(0);

		_listLapTime.Add(Time.time);

		_lap++;

		if(!GameData.Instance) {
			Debug.Log("NO GAMEDATA OBJECT");
			return;
		}

		int numLap = GameData.Instance.GetNumLap();

		if(numLap < 1) {
			Debug.Log($"ERROR: GameData _numLap: {numLap}");
			return;
		}

		if(_lap > numLap) {
			return;
		} else if(_lap == numLap && _isPly) { // dernier lap, c'est la fin du jeu
			CameraController.Instance.SetCameraMode(CameraMode.Spectate);
			if(InGameHud.Instance) {
				InGameHud.Instance.ToggleDrawHUD(false);
			}
			GetComponent<Player>().enabled = false;
			GetComponent<testbot.Bot>().enabled = true;
			// ajouter le spaceship du player dans botmanager
			if(Bot.Instance) {
				Bot.Instance.AddPlayerToBots();
			}
			NumberCountdown.Instance.Position(); // c'est le NumberCountdown qui affiche la position finale (1st, 2nd, 3rd, etc.)
			// save _listLapTime si le joueur bat son record
		}

		if (!InGameHud.Instance) {
			return;
		}
		// ne pas merge ce check de condition
		if(_isPly) {
			InGameHud.Instance.ResetProgBar();
			InGameHud.Instance.UpdateLap();
			InGameHud.Instance.DrawTimeComp();
		}
	}

	// méthode publique qui retourne le nombre de lap du spaceship
	public int GetLap()
	{
		return _lap;
	}

	// utilisé dans PosManager
	public void SetPosition(int position)
	{
		_position = position;
	}

	// utilisé dans PosManager
	public int GetPosition()
	{
		return _position;
	}

	// nombre de waypoints parcourus au total
	// faut juste sort en ordre décroissant pour trouver l'ordre des spaceships (le sort se fait dans la classe PosManager)
	public int GetPosValue()
	{
		return (_lap * WaypointManager.Instance.GetNbWpt()) + _waypoint.GetWaypoint();
	}

	/********************************************
					CONTROLS
	*********************************************/

	public void Forward()
	{
		//_rb.AddForce(transform.forward * FORWARD_ACCEL, ForceMode.Acceleration);
		if (_rb.velocity.magnitude < max_speed) {
			_rb.AddForce(transform.forward * _accel /* * (_slower + 1)*/);
		}
		if(fire) {
			fire.SetActive(true);
		}
	}

	public void Backward()
	{
		//_rb.AddForce(transform.forward * BACKWARD_ACCEL, ForceMode.Acceleration);
		_rb.AddForce(-1 * transform.forward * _accel /* * (_slower + 1)*/);
	}

	public void Turn(bool left)
	{
		_rb.AddTorque(transform.up * _agility * (left ? -1 : 1), ForceMode.Acceleration);
	}

	public void AirBrake(bool left)
	{
		_rb.AddTorque(transform.up * _airbrakePower * (left ? -1f : 1f) /* _rb.velocity.magnitude*/, ForceMode.Acceleration);

		Vector3 force = -1 * _rb.velocity * _airbrakePower / _weight;

		// division par 0 donne des erreurs
		if(!IsVecValid(force)) {
			return;
		}

		_rb.AddForce(force * 6f);
	}

	public void Orbit()
	{
		Debug.Log(Orbite.Instance.GetListCount());
		for(int i=0; i<Orbite.Instance.GetListCount(); i++) 
		{
			if((Orbite.Instance.GetOrbitePos(i) - transform.position).magnitude < 5)
			{
                Debug.DrawLine(transform.position, Orbite.Instance.GetOrbitePos(i), Color.green, Time.fixedDeltaTime);
				Vector3 direction = (Orbite.Instance.GetOrbitePos(i) - transform.position);
				direction.Normalize();

				_rb.AddForce(direction * Orbite.Instance.GetOrbiteScale(i) * 5);
            }
		}
	}

	/********************************************
					POWER UPS
	*********************************************/

	public void UsePU()
	{
		if (!_isFrozen && _pu != -1) {
			poly.PU pu = Instantiate(_gm.GetGameObjectPU(_pu), transform.position + (transform.forward * _size), Quaternion.LookRotation(transform.forward)).GetComponent<poly.PU>();
			pu.SetOwner(transform);
			_pu = -1;
			if(InGameHud.Instance) {
				InGameHud.Instance.Item(_pu);
			}
		}
	}

	// méthode publique qui donne un item au spaceship
	// sélectionne l'item en fonction de la position
	// y = a * ( x - 0.5 ) + 0.5
	public void GivePU()
	{
		int numShip = _gm.GetNumSpaceships();
		int pos = _position;
		int numPU = _gm.GetNumPUs();
		int[] listWeight = new int[numPU];
		float ratioPos = ((1f - (pos / (float)(numShip - 1))) * 2f) - 1f;

		if(numShip < 2) {
			_pu = Random.Range(0, numPU); // min inclusive et max exclusif https://docs.unity3d.com/ScriptReference/Random.Range.html#:~:text=public%20static%20int%20Range(int%20minInclusive%2C%20int%20maxExclusive)%3B
			Debug.Log($"PU PICKED: {_gm.GetGameObjectPU(_pu).name.Substring(3)}");
			if(_isPly) {
				if(InGameHud.Instance) {
					InGameHud.Instance.Item(_pu);
				}
			} else {
				UsePU();
			}
			return;
		}

		// distribution de poids
		for (int i = 0; i < numPU; i++) {
			listWeight[i] = Mathf.RoundToInt((ratioPos * ((i / (float)(numPU - 1)) - 0.5f) + 0.5f) * numPU * 100f);
		}

		/*Debug.Log($"position: {pos}");
		for(int i = 0; i < listWeight.Length; i++) {
			Debug.Log($"{i} : {listWeight[i]}");
		}*/

		int somme = listWeight.Sum();

        Debug.Log($"somme: {somme}");

		// algorithme de sélection aléatoire pondérée 
		int random = Random.Range(0, somme);

		for (int i = 0; i < numPU; i++) {
			if (random < listWeight[i]) {
				_pu = i;
				Debug.Log($"PU PICKED: {_gm.GetGameObjectPU(_pu).name.Substring(3)}");
				if(_isPly) {
					if(InGameHud.Instance) {
						InGameHud.Instance.Item(_pu);
					}
				} else {
					UsePU();
				}
				return;
			}
			random -= listWeight[i];
		}
	}

	/********************************************
						STATS
	*********************************************/

	public int GetMaxHP() {	return MAX_HP; }

	public int GetHP() { return _hp; }

	public void GiveHP(int life) { _hp = Mathf.Clamp(_hp + life, 0, MAX_HP); }

	public float GetBoost() { return _boost; }

	public void SetBoost(float boost) { _boost = Mathf.Clamp(boost, 0, _maxBoost); } 

	public float GetAgility() { return _agility; }

	public float GetMaxSpeed() { return max_speed; }

	public void SetMaxSpeed(float maxSpeed) { max_speed = maxSpeed; }

	public float GetSize() { return _size; }

	public float GetForwardSpeed() { return _forwardSpeed.magnitude; }

	// utilisé dans PUBox
	// pour éviter de désactiver le PUBox quand le spaceship a déjà un PU
	public int GetPU() { return _pu; }

	/********************************************
						AUTRE
	*********************************************/

	// méthode publique qui permet de freeze/unfreeze le spaceship
	public void Freeze(bool isFrozen) { _isFrozen = isFrozen; }

	// méthode publique pour savoir si le spaceship est frozen
	public bool isFrozen() { return _isFrozen; }

	// méthode publique qui retourne la direction de la gravité
	// utilisé dans OutOfBounds
	public Vector3 GetVecGrav() { return _rayDir; }

	// méthode publique qui retourne un bool true si c'est le joueur
	public bool IsPlayer() { return _isPly; }

	// méthode publique qui retourne le temps depuis que le dernier lap a été complété
	public float GetTimeSinceLastLap()
	{
		float timeSinceLastLap = Time.time;
		timeSinceLastLap -= _listLapTime.Count == 0 ? GameManager.Instance.GetStartTime() : _listLapTime[_listLapTime.Count - 1];
		return timeSinceLastLap;
	}

	// méthode publique qui retourne le temps du dernier lap complété
	public float GetLastLapTime()
	{
		float timeWhenLastLap = _listLapTime[_listLapTime.Count - 1];
		timeWhenLastLap -= _listLapTime.Count == 1 ? GameManager.Instance.GetStartTime() : _listLapTime[_listLapTime.Count - 2];
		return timeWhenLastLap;
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

	private bool IsVecValid(Vector3 vec)
	{
		// TODO: portes logiques peuvent être simplifié ici, pourquoi pas !( isNaN || isNaN || isNaN || IsInf || IsInf || IsInf )
		return !(float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z)) && !(float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z));
	}
}