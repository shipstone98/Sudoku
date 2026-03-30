// swift-tools-version: 6.3

import PackageDescription

let package = Package(
    name: "Sudoku",
    products: [.library(name: "Sudoku", targets: ["Sudoku"])],
    targets: [
        .target(name: "Sudoku"),
        .testTarget(name: "SudokuTests", dependencies: ["Sudoku"])
    ],
    swiftLanguageModes: [.v6]
)
