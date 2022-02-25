#pragma once
#include "stdafx.h"
#include "DebuggerFeatures.h"

enum class StepType;
class BreakpointManager;
class CallstackManager;
class IAssembler;
class BaseEventManager;
class CodeDataLogger;
class ITraceLogger;
class PpuTools;
struct BaseState;
enum class EventType;
enum class MemoryOperationType;

class IDebugger
{
public:
	bool IgnoreBreakpoints = false;
	bool AllowChangeProgramCounter = false;

	virtual ~IDebugger() = default;

	virtual void Step(int32_t stepCount, StepType type) = 0;
	virtual void Reset() = 0;
	virtual void Run() = 0;
	
	virtual void Init() {}
	virtual void ProcessConfigChange() {}

	virtual void ProcessInterrupt(uint32_t originalPc, uint32_t currentPc, bool forNmi) {}

	virtual DebuggerFeatures GetSupportedFeatures() { return {}; }
	
	virtual uint32_t GetProgramCounter(bool getInstPc) = 0;
	virtual void SetProgramCounter(uint32_t addr) = 0;

	virtual BreakpointManager* GetBreakpointManager() = 0;
	virtual CallstackManager* GetCallstackManager() = 0;
	virtual IAssembler* GetAssembler() = 0;
	virtual BaseEventManager* GetEventManager() = 0;
	virtual CodeDataLogger* GetCodeDataLogger() = 0;
	virtual ITraceLogger* GetTraceLogger() = 0;
	virtual PpuTools* GetPpuTools() { return nullptr; }

	virtual BaseState& GetState() = 0;
	virtual void GetPpuState(BaseState& state) {};
	virtual void SetPpuState(BaseState& state) {};
};
