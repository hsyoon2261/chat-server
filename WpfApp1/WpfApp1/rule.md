
Connection Request/Res = 1 접속 // 상태

Fail = 1001 

Create Request = 2 가입 =>접속전 처리는 클라 에서 처리(by enum) //상태

Fail = 1002

Login Request = 3 로그인 => 접속 전 클라 //상태

Fail - PassWord res= 1003

Fail - Other People res = 2003

Fail - Send Another Login Response 3003

Log out Request = 4 로그 아웃 => 로그인 전 // 상태 + 하위목록 쳐내기

Fail 1004

Enter Room Request = 5  방입장 => 로그인 전 // 상태

Fail(200명) 1005

Connection End request = 10 접속끊기  //상태 + 하위목록 쳐내기

Quit Room Request = 6 방 나가기 => in chat //상태

~~Fail = 1006

Chatting Request = 7 채팅 => in chat //상태

Fail = 1007



class 0 1 2 3 4 설정하고 req 오름차순 res 내림차순으로 정렬하자. 


### 서버에 들고있어야 할 항목
- <key, value>
- <소켓, 정보클래스>
- <userid, password>
- <방번호, 소켓리스트> => 개수도 세야함 . 

### 정보 클래스
- 현재 상태 => 최초/접속/로그인/방
- 접속중인 아이디 명
- 접속중인 방 


아무래도 접속 끊기 전에 패킷을 보내야겠다. 나 접속 끊어요~~~~~

소켓이 닫히기 전에 모든 데이터를 보내고 받으려면 Disconnect 메서드를 호출하기 전에 Shutdown을 호출해야 합니다.













