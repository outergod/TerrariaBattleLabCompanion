{
  description = "Project dev shell";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs, fenix, ... }:
    let
      system = "x86_64-linux"; # change as needed
      pkgs = import nixpkgs { inherit system; config.allowUnfree = true; };
    in {
      devShells.${system}.default = pkgs.mkShell {
        packages = with pkgs; [
          omnisharp-roslyn
          nodejs_24
          bun
          gh
        ];

        shellHook = ''
          export LD_LIBRARY_PATH=${pkgs.stdenv.cc.cc.lib}/lib:$LD_LIBRARY_PATH
          unset SOURCE_DATE_EPOCH
        '';
      };
    };
}
