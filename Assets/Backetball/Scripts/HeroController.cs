namespace XFramework.Backetball
{
	using UnityEngine;
	using Spine.Unity;

	[RequireComponent(typeof(Rigidbody2D))]
	public class HeroController : MonoBehaviour
	{
		[Header("Controls")]
		public string XAxis = "Horizontal";
		public string YAxis = "Vertical";
		public string JumpButton = "Jump";
		[Header("Moving")]
		public float walkSpeed = 1.5f;
		public float runSpeed = 7f;
		public float gravityScale = 6.6f;
		[Header("Jumping")]
		public float jumpSpeed = 25;
		public float jumpDuration = 0.5f;
		public float jumpInterruptFactor = 100;
		public float forceCrouchVelocity = 25;
		public float forceCrouchDuration = 0.5f;
		public bool isJumping = false;
		[Header("Graphics")]
		public SkeletonAnimation skeletonAnimation;
		[Header("Animation")]
		[SpineAnimation(dataField: "skeletonAnimation")]
		public string walkName = "Walk";
		[SpineAnimation(dataField: "skeletonAnimation")]
		public string runName = "Run";
		[SpineAnimation(dataField: "skeletonAnimation")]
		public string idleName = "Idle";
		[SpineAnimation(dataField: "skeletonAnimation")]
		public string jumpName = "Jump";
		[SpineAnimation(dataField: "skeletonAnimation")]
		public string fallName = "Fall";
		[SpineAnimation(dataField: "skeletonAnimation")]
		public string crouchName = "Crouch";
		[Header("Effects")]
		public AudioSource jumpAudioSource;
		public AudioSource hardfallAudioSource;
		public AudioSource footstepAudioSource;
		public ParticleSystem landParticles;
		[SpineEvent]
		public string footstepEventName = "Footstep";
		Rigidbody2D controller;
		Vector3 velocity = default(Vector3);
		float jumpEndTime = 0;
		bool jumpInterrupt = false;
		float forceCrouchEndTime;
		Vector2 input;
		bool wasGrounded = false;

		void Awake()
		{
			controller = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			skeletonAnimation.AnimationState.Event += HandleEvent;
		}

		void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
		{
			footstepAudioSource.Stop();
			footstepAudioSource.pitch = GetRandomPitch(0.2f);
			footstepAudioSource.Play();
		}

		static float GetRandomPitch(float maxOffset)
		{
			return 1f + Random.Range(-maxOffset, maxOffset);
		}

		void Update()
		{
			input.x = Input.GetAxis(XAxis);
			input.y = Input.GetAxis(YAxis);
			velocity = default(Vector3);
			float dt = Time.deltaTime;

			// 先不处理跳跃
			if (Input.GetButtonDown(JumpButton) && !isJumping)
			{
				jumpAudioSource.Stop();
				jumpAudioSource.Play();
				// velocity.y = jumpSpeed;
				isJumping = true;
				jumpEndTime = Time.time + jumpDuration;
			}
			else
			{
				jumpInterrupt |= Time.time < jumpEndTime && Input.GetButtonUp(JumpButton);
			}

			// 如果Y轴没有输入
			if (Mathf.Abs(input.y) < 0.1)
			{
				velocity.x = input.x;
			}
			else
			{
				velocity.x = input.x;
				velocity.y = input.y;
			}

			controller.AddForce(velocity*dt);
			float absInputX = Mathf.Abs(input.x);
			float absInputY = Mathf.Abs(input.y);
			if (absInputX < 0.1f && absInputY < 0.1f)
			{
				skeletonAnimation.AnimationName = idleName;
			}
			else if (absInputX > 0.6f || absInputY > 0.6)
			{
				skeletonAnimation.AnimationName = runName;
			}
			else
			{
				skeletonAnimation.AnimationName = walkName;
			}
		}
	}
}