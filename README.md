# 3D Space Division

3D 공간 분할과 경로 탐색 실험 (Unity 미니 프로젝트, 2023).

## 영상

[![시연 영상](https://img.youtube.com/vi/jLFEWxqg5MA/hqdefault.jpg)](https://youtu.be/jLFEWxqg5MA)

## 방식

| 케이스 | 방식 |
|---|---|
| Case 1 | 공간을 격자로 나누고 레이캐스트로 통과 가능 영역을 판정, 공간 단위 A* 탐색 |
| Case 2 · 3 | 장애물이 있는 공간만 8등분(옥트리)해 최소 크기까지 분할, 리프 공간 사이로 경로 탐색 |
