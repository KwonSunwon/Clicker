using static Util;

public class MineMapper
{
    public const int CURRENT_VERSION = 1;

    public static MineSaveDTO ToDTO(MineState state)
    {
        var dto = new MineSaveDTO {
            Id = state.Id,
            CurrentDepth = state.CurrentDepth,
            Lines = new()
        };

        foreach (var line in state.Lines) {
            var lineDTO = new LineSaveDTO {
                Depth = line.Depth,
                IsTopLine = line.IsTopLine,
                Rocks = new(),
                Veins = new()
            };

            foreach (var rock in line.Rocks) {
                var rockDTO = new RockSaveDTO {
                    Id = GetHex(rock.Id),
                    Hp = rock.Hp
                };
                lineDTO.Rocks.Add(rockDTO);
            }

            foreach (var vein in line.Veins) {
                var veinDTO = new VeinSaveDTO {
                    Id = GetHex(vein.Id),
                    Pos = GetHex(vein.Pos),
                    Type = vein.Type
                };
                lineDTO.Veins.Add(veinDTO);
            }

            dto.Lines.Add(lineDTO);
        }
        return dto;
    }

    public static bool FromDTO(MineSaveDTO dto, ref MineState state)
    {
        try {
            state.Id = dto.Id;
            state.CurrentDepth = dto.CurrentDepth;
            state.Lines = new();

            foreach (var lineDTO in dto.Lines) {
                var line = new LineState {
                    Depth = lineDTO.Depth,
                    IsTopLine = lineDTO.IsTopLine,
                    Rocks = new(),
                    Veins = new()
                };

                foreach (var rockDTO in lineDTO.Rocks) {
                    var rock = new RockState {
                        Id = GetDec(rockDTO.Id),
                        Hp = rockDTO.Hp
                    };
                    line.Rocks.Add(rock);
                }

                foreach (var veinDTO in lineDTO.Veins) {
                    var vein = new VeinState {
                        Id = GetDec(veinDTO.Id),
                        Pos = GetDec(veinDTO.Pos),
                        Type = veinDTO.Type
                    };
                    line.Veins.Add(vein);
                }

                state.Lines.Add(line);
            }
            return true;
        }
        catch (System.Exception e) {
            UnityEngine.Debug.LogError($"@MineMapper FromDTO failed: {e}");
            return false;
        }
    }
}
