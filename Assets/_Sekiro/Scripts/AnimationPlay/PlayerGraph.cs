using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

public class PlayerGraph : MonoBehaviour
{
    public Animator animator;  // 模型上的 Animator 組件

    // 你想播放的動畫片段
    public AnimationClip clip0;
    public AnimationClip clip1;

    public AudioSource audioSource;

    private PlayableGraph _playableGraph;

    [SerializeField]
    List<AudioClip> AudioClips;

    [SerializeField]
    AudioClip oneClip;
    private AnimationClipPlayable _clipPlayable;

    void Start()
    {
        // 創建 PlayableGraph
        _playableGraph = PlayableGraph.Create("ModelAnimationGraph");

        OneMotion();
        // AudioPlay();
        TwoMotionMixer();
    }

    // private async Task AudioPlay()
    // {
    //     var playableGraphAudio = PlayableGraph.Create("Graph");

    //     /*要讀標籤，讀群組沒用*/
    //     var b = Addressables.LoadAssetsAsync<AudioClip>("Music", (x) =>
    //     {
    //         Debug.Log($"讀取完畢{x.name}");
    //     });
    //     // 讀取群組 會讀出一堆null
    //     // var a = Addressables.LoadAssetsAsync<AudioClip>("New", (x) =>
    //     // {
    //     //     Debug.Log($"讀取完畢{x.name}");
    //     // });
    //     var result = await b.Task;

    //     AudioClips = new List<AudioClip>(result);

    //     // 讀取單首
    //     oneClip = await Addressables.LoadAssetAsync<AudioClip>("BGM0").Task;


    //     var audioClipPlayable = AudioClipPlayable.Create(playableGraphAudio, oneClip, false); // true 代表循環播放

    //     // 就像動畫一樣，它也需要一個 Output 接出去
    //     var output2 = AudioPlayableOutput.Create(playableGraphAudio, "AudioOut", audioSource);
    //     output2.SetSourcePlayable(audioClipPlayable);
    //     playableGraphAudio.Play();
    // }

    private void OneMotion()
    {
        // 創建 AnimationPlayableOutput 並連接到 Animator
        var output = AnimationPlayableOutput.Create(_playableGraph, "Animation", animator);

        // 創建 AnimationClipPlayable，將動畫片段載入 Playable
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip1);

        // 將 AnimationClipPlayable 連接到輸出
        output.SetSourcePlayable(clipPlayable);

        // 啟動 PlayableGraph
        _playableGraph.Play();
    }

    private void TwoMotionMixer()
    {
        // 1. 創建 Mixer
        var mixer = AnimationMixerPlayable.Create(_playableGraph, 2); // 準備 2 個插槽

        // 2.創建 AnimationPlayableOutput 並連接到 Animator
        var output = AnimationPlayableOutput.Create(_playableGraph, "Animation", animator);

        // 3. 創建兩捲錄影帶並插到 Mixer 上
        var idleClip = AnimationClipPlayable.Create(_playableGraph, clip0);
        var walkClip = AnimationClipPlayable.Create(_playableGraph, clip1);

        _playableGraph.Connect(idleClip, 0, mixer, 0); // 錄影帶 0 號插到 Mixer 的 0 號口
        _playableGraph.Connect(walkClip, 0, mixer, 1); // 錄影帶 1 號插到 Mixer 的 1 號口

        // 4. 把 Mixer 接到螢幕 (Output)
        output.SetSourcePlayable(mixer);

        // 5. 動態調權重 (這就是你可以發揮騷操作的地方)
        mixer.SetInputWeight(0, 0.5f); // Idle 佔 50%
        mixer.SetInputWeight(1, 0.5f); // Walk 佔 50%

        // // 也可以用DoTween做動態權重
        // DOVirtual.Float(0f, 1f, 1f, (v) =>
        // {
        //     mixer.SetInputWeight(0, 1f - v); // 逛街權重 1 -> 0
        //     mixer.SetInputWeight(1, v);      // 抽獎權重 0 -> 1
        // }).SetEase(Ease.InOutQuad);

        // 啟動 PlayableGraph
        _playableGraph.Play();
    }

    public void ManualUpdate(float time)
    {
        if (!_playableGraph.IsValid())
        {
            _playableGraph = PlayableGraph.Create("EditorGraph");
            var output = AnimationPlayableOutput.Create(_playableGraph, "Anim", animator);

            // 這裡要把生成的 Playable 存起來
            _clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip1);
            output.SetSourcePlayable(_clipPlayable);
        }

        // ✅ 正確做法：對「節點」設定時間，而不是對 Graph
        _clipPlayable.SetTime(time);

        // ✅ 然後叫 Graph 根據新的時間計算（Evaluate）
        _playableGraph.Evaluate();
    }

    public void OnDestroy()
    {
        // 銷毀 PlayableGraph 以釋放資源
        _playableGraph.Destroy();
    }
}