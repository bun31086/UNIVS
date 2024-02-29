// ---------------------------------------------------------  
// EnemyManager.cs  
// �G�S�̊Ǘ��X�N���v�g
// �쐬��:  2/22
// �쐬��:  �|�����l
// ---------------------------------------------------------  
using UnityEngine;
using UniRx;

public class EnemyManager : MonoBehaviour,IDamageable
{
    #region �ϐ�

    #region �X�N���v�g�n

    [Tooltip("�G���苓���X�N���v�g")]
    private EnemyRun _enemyRun = default;
    [Tooltip("�v���C���[�̕����ɓG����������X�N���v�g")]
    private EnemyLookat _enemyLookat = default;
    [Tooltip("�G�A�j���[�V�����Ǘ��X�N���v�g")]
    private EnemyAnimation _enemyAnimation = default;
    [Tooltip("�U���q�b�g�󂯎��X�N���v�g(BoxCollider)")]
    private EnemyTrigger _enemyTriggerBox = default;
    [Tooltip("�U���q�b�g�󂯎��X�N���v�g(CapselCollider)")]
    private EnemyTrigger _enemyTriggerBall = default;
    [Tooltip("�G���ړ��X�N���v�g")]
    private EnemySideMove _enemySideMove = default;
    [Tooltip("�G�X�e�[�^�X�Ǘ��X�N���v�g")]
    private EnemyStatus _enemyStatus = new EnemyStatus();
    [Tooltip("���G�X�N���v�g")]
    private EnemySearch _enemySearch = new EnemySearch();
    [Tooltip("�v���C���[��ԊǗ��X�N���v�g")]
    private readonly ReactiveProperty<EnemyState> _enemyState = new ReactiveProperty<EnemyState>(EnemyState.Idle);
    //private EnemyStun _enemyStun = new EnemyStun(); ������
    #endregion

    #region �萔
    [Tooltip("�G�̍��G�͈�")]
    private const int CONST_SEARCH_RANGE = 30;
    [Tooltip("�G�̍U���\�͈�")]
    private const int CONST_ATTACK_RANGE = 4;

    #endregion

    [Tooltip("�G�ƃv���C���[�̋���")]
    private float _distance = default;
    [Tooltip("�v���C���[�̍��W")]
    private Vector3 _playerPosition = default;
    [Tooltip("�G�̍��W")]
    private Vector3 _enemyPosition = default;

    private Transform _playerTransform = default;
    private Transform _enemyTransform = default;
    private Rigidbody _enemyRigidbody = default;
    private Animator _enemyAnimator = default;

    [SerializeField,Tooltip("�v���C���[�I�u�W�F�N�g")]
    private GameObject _player;
    [SerializeField,Tooltip("�G�̍U���͈̓I�u�W�F�N�g(��)")]
    private GameObject _triggerBoxObject = default;
    [SerializeField, Tooltip("�G�̍U���͈̓I�u�W�F�N�g(��)")]
    private GameObject _triggerBallObject = default;
    [Tooltip("�U�����Ă��邩")]
    private bool _isAttack = false;
    [Tooltip("�U�����n�߂���")]
    private bool _isAttackFirst = false;
    [Tooltip("���ړ����Ă��邩")]
    private bool _isSideMove = false;
    [Tooltip("���ړ����n�߂���")]
    private bool _isSideMoveFirst = false;
    [Tooltip("����̓����蔻��")]
    private BoxCollider _boxCollider = default;
    [Tooltip("�E��̓����蔻��")]
    private CapsuleCollider _capsuleCollider = default;
    [Tooltip("1���� 2���E�Ɉړ�����")]
    private int _moveDirection = default;


    #endregion

    #region �v���p�e�B

    /// <summary>
    /// EnemyUIPresenter��EnemyHP��n������
    /// </summary>
    public EnemyStatus EnemyStatus { get => _enemyStatus; set => _enemyStatus = value; }

    #endregion

    #region ���\�b�h  

    /// <summary>  
    /// ����������  
    /// </summary>  
    void Awake()
    {
        _enemyTriggerBox = _triggerBoxObject.GetComponent<EnemyTrigger>();
        _enemyTriggerBall = _triggerBallObject.GetComponent<EnemyTrigger>();
        _enemyAnimator = this.GetComponent<Animator>();
        _enemyAnimation = new EnemyAnimation(_enemyAnimator);

        //HP������������
        EnemyStatus.EnemyHP.Value = EnemyStatus.EnemyMaxHp;

        #region �C�x���g
        EnemyStatus.EnemyHP
            //�G��HP��0�ɂȂ����Ƃ�
            .Where(_ => EnemyStatus.EnemyHP.Value <= 0)
            .Subscribe(_ =>
            {
                //��Ԃ����S��Ԃɂ���
                _enemyState.Value = EnemyState.Death;
                //�A�j���[�V������ύX����
                _enemyAnimation.Animation((int)_enemyState.Value, 0);
                //�����蔻����Ȃ���
                _capsuleCollider.enabled = false;
                _boxCollider.enabled = false;
            }).AddTo(this);
        _enemyState
            //�U����ԁA���S��ԁA���ړ��ȊO�̂Ƃ�
            .Where(_ => _enemyState.Value != EnemyState.Attack && _enemyState.Value != EnemyState.Death && _enemyState.Value != EnemyState.SlowMove)
            .Subscribe(_ =>
            {
                //�A�j���[�V������ύX����
                _enemyAnimation.Animation((int)_enemyState.Value, 0);
            }).AddTo(this);
        //�U���������������iBoxCollider�j
        _enemyTriggerBox.IsHit
            .Subscribe(_ =>
            {
                //����IDamageable�������Ă�����
                if (_enemyTriggerBox.HitObject.TryGetComponent(out IDamageable damageable))
                {
                    //���������I�u�W�F�N�g�̃C���^�[�t�F�[�X���Q�Ƃ��A�_���[�W��^����
                    _enemyTriggerBox.HitObject.GetComponent<IDamageable>().DamageHit(EnemyStatus.EnemyPower);
                }
            });
        //�U���������������iCapselCollider�j
        _enemyTriggerBall.IsHit
            .Subscribe(_ =>
            {
                //����IDamageable�������Ă�����
                if (_enemyTriggerBall.HitObject.TryGetComponent(out IDamageable damageable))
                {
                    //���������I�u�W�F�N�g�̃C���^�[�t�F�[�X���Q�Ƃ��A�_���[�W��^����
                    _enemyTriggerBall.HitObject.GetComponent<IDamageable>().DamageHit(EnemyStatus.EnemyPower);
                }
            });

        #endregion
    }

    /// <summary>  
    /// �X�V�O����  
    /// </summary>  
    void Start()
    {
        _playerTransform = _player.transform;
        _enemyTransform = this.transform;
        _enemyRigidbody = this.GetComponent<Rigidbody>();
        _boxCollider = _triggerBoxObject.GetComponent<BoxCollider>();
        _capsuleCollider = _triggerBallObject.GetComponent<CapsuleCollider>();
        #region �C���X�^���X��
        _enemyRun = new EnemyRun(_enemyTransform, _enemyRigidbody);
        _enemyLookat = new EnemyLookat(_enemyTransform);
        _enemySideMove = new EnemySideMove(_enemyTransform, _enemyRigidbody);
        #endregion

    }

    /// <summary>  
    /// �X�V����  
    /// </summary>  
    void Update()
    {
        UpdateManager();
    }

    /// <summary>
    /// �X�V�����i�������Z�j
    /// </summary>
    private void FixedUpdate()
    {
        FixedManager();
    }

    /// <summary>
    /// Update��{����
    /// </summary>
    private void UpdateManager()
    {
        //���S���Ă�����
        if (_enemyState.Value == EnemyState.Death)
        {
            //�����𒆒f
            return;
        }

        //�U����Ԃ��X�^����Ԃ����S��ԈȊO�Ȃ�
        if (_enemyState.Value != EnemyState.Attack && _enemyState.Value != EnemyState.Stun && _enemyState.Value != EnemyState.Death)
        {
            //�v���C���[�̍��W�𒲂ׂ�
            _playerPosition = _playerTransform.position;
            //�G�̍��W�𒲂ׂ�
            _enemyPosition = _enemyTransform.position;
            //�v���C���[�ƓG�̋����𒲂ׂ�
            _distance = _enemySearch.Search(_playerPosition, _enemyPosition);
        }

        switch (_enemyState.Value)
        {
            //�ҋ@���
            case EnemyState.Idle:          
                //���G�͈͓��ɓ�������Run����
                if (_distance <= CONST_SEARCH_RANGE)
                {
                    _enemyState.Value = EnemyState.Run;
                }
                break;
            //�_�b�V�����
            case EnemyState.Run:
                //��Ƀv���C���[�̕�������
                _enemyLookat.Lookat(_playerPosition, _enemyPosition);
                //�U���͈͓��ɓ�������U�����J�n
                if (_distance <= CONST_ATTACK_RANGE)
                {
                    //���̍s���͍U��������ނ��烉���_���őI�o������s����
                    _enemyState.Value = EnemyState.Attack;
                }
                break;
            //�U�����
            case EnemyState.Attack:
                //�v���C���[�̕�������
                _enemyLookat.Lookat(_playerPosition, _enemyPosition);
                // �U���ɑJ�ڂ��Ă���ŏ��̈�x�̂ݎ��s
                if (!_isAttackFirst)
                {                
                    //�U���t���O���I���ɂ���
                    _isAttackFirst = true;
                    _isAttack = true;
                    //�U���̎�ނ������_���Ɉ���߂�
                    int attackRandom = Random.Range(1, 6);
                    //���߂�ꂽ�U���̃A�j���[�V���������s
                    _enemyAnimation.Animation((int)_enemyState.Value, attackRandom);
                }
                //�U���I����Ɏ��̍s�������s����
                if (!_isAttack)
                {
                    _isAttackFirst = false;
                    //���̍s���́i�ҋ@���1�A�U���������2�A���ړ�3�j���烉���_���őI�o������s����
                    int stateRandom = Random.Range(1, 4);
                    _enemyState.Value = (EnemyState)stateRandom;
                }
                break;
            //���ړ����
            case EnemyState.SlowMove:
                //�U������G���A������
                _boxCollider.enabled = false;
                _capsuleCollider.enabled = false;
                //�v���C���[�̕�������
                _enemyLookat.Lookat(_playerPosition, _enemyPosition);

                // ���ړ��ɑJ�ڂ��Ă���ŏ��̈�x�̂ݎ��s
                if (!_isSideMoveFirst)
                {
                    //���ړ��t���O�𗧂Ă�
                    _isSideMoveFirst = true;
                    _isSideMove = true;
                    //�����_���ŉE�ړ������ړ������߂�
                    int leftRight = Random.Range(1, 3);
                    //���܂����A�j���[�V���������s
                    _enemyAnimation.Animation((int)_enemyState.Value, leftRight);
                    //FiexdUpdate�ɘA�g����
                    _moveDirection = leftRight;
                }
                //�I������A���̍s�����J�n
                if (!_isSideMove)
                {
                    _isSideMoveFirst = false;
                    //���̍s���͍U��������ނ��A�ҋ@��Ԃ��烉���_���őI�o������s����
                    int stateRandom = Random.Range(1, 3);
                    _enemyState.Value = (EnemyState)stateRandom;
                    //Fiexd�ɃA�j���[�V�����I����`����
                    _moveDirection = default;
                }
                break;
            //�X�^����ԁi�������j
            case EnemyState.Stun:
                //�U������邽�тɒ~�ς���Ă���
                //�~�σ|�C���g�����l�ɒB����ƃX�^������
                //�~�σ|�C���g�͏�ɏ����������Ă���
                //�X�^���A�j���[�V���������s���A�A�j���[�V�����I������A�ҋ@��Ԃɂ���
                break;
        }
    }

    /// <summary>
    /// FixedUpdate��{����
    /// </summary>
    private void FixedManager()
    {
        switch (_enemyState.Value)
        {
            //������
            case EnemyState.Run:            
                //�v���C���[�̋߂��܂ő���
                _enemyRun.Run(_playerPosition);
                break;
            //������艡�ړ�
            case EnemyState.SlowMove:       
                //���ړ�������������܂��Ă�����
                if (_moveDirection != default)
                {
                    //���߂�ꂽ�����Ɉړ�����
                    _enemySideMove.SideMove(_moveDirection);
                }
                break;
        }
    }

    #region �A�j���[�^�ɌĂяo����郁�\�b�h

    /// <summary>
    /// �U���I�����ɃA�j���[�^�[����Ăяo����郁�\�b�h
    /// </summary>
    public void AttackEnd()
    {
        //�U���I����ʒm
        _isAttack = false;
    }
    /// <summary>
    /// ���ړ��I�����ɃA�j���[�^�[����Ăяo����郁�\�b�h
    /// </summary>
    public void SideMoveEnd()
    {
        //���ړ��I����ʒm
        _isSideMove = false;
    }
    /// <summary>
    /// �U���J�n���ɃA�j���[�^�[����Ăяo����郁�\�b�h
    /// </summary>
    public void AttackStart(int attackType)
    {
        //�U���A�j���[�V�����̎�ނɑΉ����������蔻����o��
        switch (attackType)
        {
            case 1:
                _capsuleCollider.enabled = true;
                break;
            case 2:
                _capsuleCollider.enabled = true;
                break;
            case 3:
                _boxCollider.enabled = true;
                break;
            case 4:
                _capsuleCollider.enabled = true;
                break;
            case 5:
                _boxCollider.enabled = true;
                _capsuleCollider.enabled = true;
                break;
        }
    }
    #endregion

    /// <summary>
    /// �G�̗̑͂��_���[�W�����炷
    /// </summary>
    /// <param name="damage">�����Ă����_���[�W��</param>
    public void DamageHit(int damage)
    {
        EnemyStatus.EnemyHP.Value -= damage;
    }

    #endregion
}
