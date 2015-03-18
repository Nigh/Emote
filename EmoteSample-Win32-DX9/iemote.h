#ifndef __EMOTE_DRIVER_H
#define __EMOTE_DRIVER_H

#include <cstddef>
#include <d3d9.h>


#ifdef EMOTE_STATIC_LIBRARY
#define _EXPORT
#else
#ifdef EMOTE_EXPORTS
#define _EXPORT __declspec(dllexport)
#else
#define _EXPORT __declspec(dllimport)
#endif // EMOTE_EXPORTS
#endif // EMOTE_STATIC_LIBRARY


typedef BYTE emote_uint8_t;
typedef ULONG emote_uint32_t;


class _EXPORT IEmoteDevice
{
public:
  virtual emote_uint32_t AddRef(void) = 0;
  virtual emote_uint32_t Release(void) = 0;

  enum mask_mode_t {
    MASK_MODE_STENCIL,
    MASK_MODE_ALPHA,
  };

  enum shader_model_t {
    SHADER_MODEL_NO_SHADER,
    SHADER_MODEL_2,
    SHADER_MODEL_3,
  };

  enum alpha_op_t {
    ALPHA_OP_DESTALPHA_MULTIPLY_INVSRCALPHA_PLUS_SRCALPHA,
    ALPHA_OP_DESTALPHA_PLUS_SRCALPHA,
  };

  typedef void* (*ObjMemAlloc)(std::size_t size);
  typedef void (*ObjMemFree)(void *ptr);
  struct ObjMemAllocator
  {
    ObjMemAlloc alloc;
    ObjMemFree free;
  };


  struct InitParam {
    LPDIRECT3DDEVICE9 d3dDevice;
    ObjMemAllocator objAllocator;
  };

  virtual void SetMaskMode(mask_mode_t maskMode) = 0;
  virtual mask_mode_t GetMaskMode(void) const = 0;

  virtual void SetMaskRegionClipping(bool state) = 0;
  virtual bool GetMaskRegionClipping(void) const = 0;

  virtual shader_model_t GetAvailableShaderModel(void) const = 0;
  virtual void SetShaderModel(shader_model_t model) = 0;
  virtual shader_model_t GetShaderModel(void) const = 0;

  virtual void SetMipMapEnabled(bool state) = 0;
  virtual bool GetMipMapEnabled(void) const = 0;

  virtual void SetAlphaOp(alpha_op_t alphaOp) = 0;
  virtual alpha_op_t GetAlphaOp(void) const = 0;

  virtual void SetProtectTranslucentTextureColor(bool state) = 0;
  virtual bool GetProtectTranslucentTextureColor(void) const = 0;

  virtual void SetPixelateDivision(int division) = 0;
  virtual int GetPixelateDivision(void) const = 0;

  virtual void SetMaxTextureSize(emote_uint32_t width, emote_uint32_t height) = 0;

  virtual void CreatePlayer(const emote_uint8_t *emoteObjectImage, emote_uint32_t emoteObjectSize, class IEmotePlayer **player) = 0;
  virtual void CreatePlayer(emote_uint32_t emoteObjectNum, const emote_uint8_t **emoteObjectImage, const emote_uint32_t *emoteObjectSize, class IEmotePlayer **player) = 0;

  virtual void OnDeviceLost(void) = 0;
  virtual void OnRenderTarget(LPDIRECT3DTEXTURE9 renderTargetTexture) = 0;
};


class _EXPORT IEmotePlayer
{
public:
  enum timeline_play_flags_t {
    TIMELINE_PLAY_PARALLEL   = 1 << 0,
    TIMELINE_PLAY_DIFFERENCE = 1 << 1,
  };

  virtual emote_uint32_t AddRef(void) = 0;
  virtual emote_uint32_t Release(void) = 0;

  virtual IEmotePlayer *Clone(void) = 0;
  virtual void AssignState(IEmotePlayer *another) = 0;

  virtual void Show(void) = 0;
  virtual void Hide(void) = 0;
  virtual bool IsHidden(void) const = 0;

  virtual void SetSmoothing(bool state) = 0;
  virtual bool GetSmoothing(void) const = 0;

  virtual void SetMeshDivisionRatio(float ratio) = 0;
  virtual float GetMeshDivisionRatio(void) const = 0;

  virtual void SetQueuing(bool state) = 0;
  virtual bool GetQueuing(void) const = 0;

  virtual void SetHairScale(float scale) = 0;
  virtual float GetHairScale(void) const = 0;

  virtual void SetPartsScale(float scale) = 0;
  virtual float GetPartsScale(void) const = 0;

  virtual void SetBustScale(float scale) = 0;
  virtual float GetBustScale(void) const = 0;

  virtual void SetCoord(float x, float y, float frameCount = 0, float easing = 0) = 0;
  virtual void GetCoord(float &x, float &y) const = 0;

  virtual void SetScale(float scale, float frameCount = 0, float easing = 0) = 0;
  virtual float GetScale(void) const = 0;

  virtual void SetRot(float rot, float frameCount = 0, float easing = 0) = 0;
  virtual float GetRot(void) const = 0;
  
  virtual void SetColor(emote_uint32_t rgba, float frameCount = 0, float easing = 0) = 0;
  virtual emote_uint32_t GetColor(void) const = 0;

  virtual emote_uint32_t CountVariables(void) const = 0;
  virtual const char *GetVariableLabelAt(emote_uint32_t variableIndex) const = 0;
  virtual emote_uint32_t CountVariableFrameAt(emote_uint32_t variableIndex) const = 0;
  virtual const char *GetVariableFrameLabelAt(emote_uint32_t variableIndex, emote_uint32_t frameIndex) const = 0;
  virtual float GetVariableFrameValueAt(emote_uint32_t variableIndex, emote_uint32_t frameIndex) const = 0;

  virtual void SetVariable(const char *label, float value, float frameCount = 0, float easing = 0) = 0;
  virtual float GetVariable(const char *label) const = 0;

  virtual void StartWind(float start, float goal, float speed, float powMin, float powMax) = 0;
  virtual void StopWind(void) = 0;

  virtual emote_uint32_t CountMainTimelines(void) const = 0;
  virtual const char *GetMainTimelineLabelAt(emote_uint32_t index) const = 0;
  virtual emote_uint32_t CountDiffTimelines(void) const = 0;
  virtual const char *GetDiffTimelineLabelAt(emote_uint32_t index) const = 0;
  virtual emote_uint32_t CountPlayingTimelines(void) const = 0;
  virtual const char *GetPlayingTimelineLabelAt(emote_uint32_t index) const = 0;
  virtual emote_uint32_t GetPlayingTimelineFlagsAt(emote_uint32_t index) const = 0;
  virtual bool IsLoopTimeline(const char *label) const = 0;
  virtual void PlayTimeline(const char *label, emote_uint32_t flags = 0) = 0;
  virtual bool IsTimelinePlaying(const char *label = "") const = 0;
  virtual void StopTimeline(const char *label = "") = 0;
  virtual void SetTimelineBlendRatio(const char *label, float value, float frameCount = 0, float easing = 0, bool stopWhenBlendDone = false) = 0;
  virtual float GetTimelineBlendRatio(const char *label) const = 0;
  virtual void FadeInTimeline(const char *label, float frameCount = 0, float easing = 0) = 0;
  virtual void FadeOutTimeline(const char *label, float frameCount = 0, float easing = 0) = 0;
  
  virtual bool IsAnimating(void) const = 0;
  virtual void Skip(void) = 0;

  //ADDED: only for nekopara version E-mote
#if EMOTE_NEKOPARA
  virtual void UnknownFunc(void) = 0;
#endif

  virtual void Progress(float frameCount) = 0;
  virtual bool IsModified(void) const = 0;
  virtual void Render(void) = 0;
};


typedef IEmoteDevice* LPEMOTEDEVICE;
typedef IEmoteDevice* PEMOTEDEVICE;


typedef IEmotePlayer* LPEMOTEPLAYER;
typedef IEmotePlayer* PEMOTEPLAYER;


_EXPORT IEmoteDevice *EmoteCreate(const IEmoteDevice::InitParam &param);


typedef void(*FP_EMOTE_FILTER_FUNC)(emote_uint8_t *image, emote_uint32_t imageSize);

_EXPORT void EmoteFilterTexture(emote_uint8_t *image, emote_uint32_t imageSize, FP_EMOTE_FILTER_FUNC filterFunc);


#endif // __EMOTE_DRIVER_H
