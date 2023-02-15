# MovementSystem
My Movement System

모듈 형태로 되어있는 이동 시스템.


MovementSystem에 Modifier를 등록하여 이동을 처리함.

자체적으로 작동되지 않고, 이동의 처리 부분은 직접 추가해야함.( Rigidbody, Character Controller etc...)

또, Update가 되지 않기에 스크립트에서 UpdateMovement(float deltaTime) 를 수동으로 작동시켜줘야함.


기본적인 이동, 중력, 힘, 접지 Modifier가 있음.


https://github.com/kimjisoo4/MyUtilities 가 필요함.


- 사용은 자유이나 그로 인해 생긴 오류에 대해서는 책임지지 않음.

자세한 정보 : https://jisooworkstation.notion.site/Ability-System-cb0e1a9dd7e742e5b704efb0386d17a9
