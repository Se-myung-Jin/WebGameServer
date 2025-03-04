namespace Common;

public enum EResult
{
    Error_Unknown = 0,
    Error_Internal,
    Error_NoService,
    Error_EmptyPlayer,
    Error_LoadRequiredData,
    Error_UserNotFound,
    Error_UnknownChannelType,
    Error_TimeOut,
    Error_UnknownUseItemType,

    Success = 100,
    /// <summary>
    /// 잘못된 파라미터
    /// </summary>
    Error_InvalidParameter = 1000,
    /// <summary>
    /// 캐싱된 계정 정보를 찾을수 없음 다시 로그인 필요.
    /// </summary>
    Error_CacheAccountNotFound,
    /// <summary>
    /// 계정 및 패스워드등 문제 있음
    /// </summary>
    Error_InvalidIdOrPass,
    /// <summary>
    /// 외부 인증 실패
    /// </summary>
    Error_ThridPartyValidateFail,
    /// <summary>
    /// 확인 불가능한 인증키
    /// </summary>
    Error_NotFoundAuthKey,
    /// <summary>
    /// 계정 블럭중
    /// </summary>
    Error_Block,
    /// <summary>
    /// 현재 이용 불가능 서버임으로 서버 목록을 갱신해 주세요.
    /// </summary>
    Error_NotAvailableWorld,
    /// <summary>
    /// 이미 월드를 선택했는데 추가로 선택하려고 하는경우.(다시 고려)
    /// </summary>
    Error_AlreadyWorldSelect,
    /// <summary>
    /// 오픈되지 않은 월드
    /// </summary>
    Error_NotOpenWorld,
    /// <summary>
    /// 진입이 가능한 서버를 찾을수 없음.
    /// </summary>
    Error_NotFoundAvailableServer,
    /// <summary>
    /// 서버 선택 데이터가 만료된 상황. 다시 로그인 필요
    /// </summary>
    Error_SelectWaitingExpire,
    /// <summary>
    /// 기존 예약과 예약키가 일치 하지 않음. 월드 선택으로 되돌아 가야함.
    /// </summary>
    Error_ReserveKeyNotMatchGoBackSelectServer,
    /// <summary>
    /// 재접속 할수 없는 상태임 로그인으로 돌아감
    /// </summary>
    Error_ReonnectFail,
    /// <summary>
    /// 중복 접속 시도
    /// </summary>
    Error_DuplicateConnect,
    /// <summary>
    /// 캐릭터 생성 제한으로 생성할 수 없음
    /// </summary>
    Error_CreateFailProhibited,
    /// <summary>
    /// 캐릭터 슬롯이 유효한 상태라 생성할수 없음.
    /// </summary>
    Error_CreateFailNotEmptySlot,
    /// <summary>
    /// 생성할수 없는 캐릭터 입니다.
    /// </summary>
    Error_CreateFailNotAvailableCharacter,
    /// <summary>
    /// 대상 캐릭터를 찾을수 없음.
    /// </summary>
    Error_PlayerSlotNotFound,
    /// <summary>
    /// 네이밍 중복
    /// </summary>
    Error_DuplicateName,
    Error_TableNotFound,
    Error_ExpireSlot,
};